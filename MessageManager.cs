using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MessageManager : MessageObject
{
    /// <summary>
    ///管理用クラス
    ///Udpのクラスもここで管理
    ///メッセージをネット側に流したりオブジェクトに流したりを一挙に請け負う
    /// </summary>
    
    public Player player;
    public GameObject online_prefab;

    private Udp udp;

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();
        udp = new Udp();
        udp.ReciveMessage += Udp_ReciveMessage;
    }

    private void Udp_ReciveMessage(object sender, System.EventArgs e)
    {
        var mes = Encoding.UTF8.GetString(udp.buf);
        Debug.Log("recive" + udp.recive_ip + mes);
        MessageListner(new Message("online",udp.recive_ip+"+"+mes));
        udp.WaitMessage();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public override void ProcessingMessage()
    {
        while(message_queue.Count != 0)
        {
            var dequeue_mes = message_queue.Dequeue();
            Debug.Log("mes:"+dequeue_mes.Order+":"+dequeue_mes.Option);

            if(dequeue_mes.Order == "key")
            {
                KeyAction(dequeue_mes.Option);
            }
            else if(dequeue_mes.Order == "send")
            {
                SendMessage(dequeue_mes);
            }
            else if(dequeue_mes.Order == "online")
            {
                OnlineAction(dequeue_mes.Option);
            }
            else if(dequeue_mes.Order == "player")
            {
                foreach (var op in FindObjectsOfType<OnlinePlayer>())
                {
                    udp.SendMessage(op.IPAdress, dequeue_mes.Option);
                }
            }
            else if(dequeue_mes.Order == "event")
            {
                foreach (var op in FindObjectsOfType<OnlinePlayer>())
                {
                    udp.SendMessage(op.IPAdress, "event:" + dequeue_mes.Option);
                }

            }
        }
    }

    private void SendMessage(Message message)
    {
        var split = message.Option.Split('>');
        var ip = split[0];
        if (ip == "*")
        {
            foreach (var op in FindObjectsOfType<OnlinePlayer>())
            {
                udp.SendMessage(op.IPAdress, split[1]);
            }
        }
        else
        {
            udp.SendMessage(ip, split[1]);
        }
    }

    private void ConnectStart(string ip)
    {
        var on_player = Instantiate(online_prefab);
        var online = on_player.GetComponent<OnlinePlayer>();
        online.IPAdress = ip;
        online.manager = this;
        online.Name.text = ip;
        udp.SendMessage(ip, "connect:complete,8,1"); //connect:complete,(相手の位置),(自分の位置)⇒今回は固定
    }

    private void ConnectComplete(string ip,string option)
    {
        var split = option.Split(',');
        //こちらのキャラの位置
        var position = int.Parse(split[1]);
        float player_x = (int)(position % 11) - 4.5f;
        float player_y = -(int)(position / 11) + 3.5f;
        player.transform.position = new Vector3(player_x, player_y, 0);

        //相手のキャラ用のオブジェクト生成
        var on_player = Instantiate(online_prefab);
        var online = on_player.GetComponent<OnlinePlayer>();
        online.IPAdress = ip;
        online.manager = this;
        online.Name.text = ip;
        position = int.Parse(split[2]);
        float online_x = (int)(position % 11) - 4.5f;
        float online_y = -(int)(position / 11) + 3.5f;
        online.transform.position = new Vector3(online_x, online_y, 0);
    }

    private void OnlineAction(string option)
    {
        var split = option.Split('+');
        var ip = split[0];
        var mes = split[1].Split(':');
        var new_order = mes[0];
        var new_option = mes[1];
        if(new_order == "connect")
        {
            if (new_option.Contains("start")) ConnectStart(ip);
            else if (new_option.Contains("complete")) ConnectComplete(ip,new_option);
        }
        else if(new_order == "move")
        {
            foreach (var op in FindObjectsOfType<OnlinePlayer>())
            {
                if(op.IPAdress == ip)
                {
                    op.MessageListner(new Message("move", new_option));
                }
            }
        }
        else if(new_order == "event")
        {
            OnlineActionEvent(ip, new_option);
        }
    }

    private void OnlineActionEvent(string ip,string option)
    {
        var option_split = option.Split(',');
        if (option_split[0] == "start")
        {
            if (option_split.Length == 1) //別プレイヤーからのイベント開始メッセージ⇒OnlinePlayerオブジェクトと同期
            {
                foreach (var op in FindObjectsOfType<OnlinePlayer>())
                {
                    if (op.IPAdress == ip)
                    {
                        op.MessageListner(new Message("event", "start"));
                    }
                }
            }
            else if (option_split[1] == "1")  //NPCが誰かとイベント開始
            {
                FindObjectOfType<NonPlayer>().MessageListner(new Message("event", "start"));
            }
            else //自キャラがイベント開始
            {
                player.MessageListner(new Message("event", "start"));
            }
        }
        else if (option_split[0] == "end")
        {
            if (option_split.Length == 1)
            {
                foreach (var op in FindObjectsOfType<OnlinePlayer>())
                {
                    if (op.IPAdress == ip)
                    {
                        op.MessageListner(new Message("event", "end"));
                    }
                }
            }
            else if (option_split[1] == "1")
            {
                FindObjectOfType<NonPlayer>().MessageListner(new Message("event", "end"));
            }
            else
            {
                player.MessageListner(new Message("event", "end"));
            }
        }
    }

    private void KeyAction(string key)
    {
        switch (key)
        {
            case "c":
                udp.SendMessage("192.168.0.106","connect:start");
                break;
            case "escape":
                Application.Quit();
                break;
        }
    }

    public List<Character> GetCharacterList()
    {
        var npc = new List<Character>(FindObjectsOfType<NonPlayer>());
        var online = new List<Character>(FindObjectsOfType<OnlinePlayer>());
        npc.InsertRange(0,online);
        return npc;
    }



}
