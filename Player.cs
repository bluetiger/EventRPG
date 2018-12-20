using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : Character
{
    private Character event_opponent;

    public new void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Z))  //イベントキー
        {
            if (!EventRunning) EventCheck();
            else EventEnd(false);
        }

        if (EventRunning) return;　//イベント中なら移動禁止

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var mes = new Message("player", "move:1");
            Move(Direction.Up);
            manager.MessageListner(mes);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            var mes = new Message("player", "move:2");
            Move(Direction.Down);
            manager.MessageListner(mes);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            var mes = new Message("player", "move:3");
            Move(Direction.Right);
            manager.MessageListner(mes);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var mes = new Message("player", "move:4");
            Move(Direction.Left);
            manager.MessageListner(mes);
        }
    }

    public override void ProcessingMessage()
    {
        while (message_queue.Count != 0)
        {
            var mes = message_queue.Dequeue();
            var order = mes.Order;
            var option = mes.Option;
            switch (order)
            {
                case "move":
                    if (option == "1") Move(Direction.Up);
                    else if (option == "2") Move(Direction.Down);
                    else if (option == "3") Move(Direction.Right);
                    else if (option == "4") Move(Direction.Left);
                    break;
                case "event": //オンラインからのイベント開始メッセージ
                    if (option == "start")
                    {
                        EventRunning = true;
                        Name.enabled = true;
                        manager.MessageListner(new Message("event", "start"));
                    }
                    else
                    {
                        EventEnd(true);
                    }
                    break;

            }
        }
    }

    private void EventCheck()  //イベント開始可能かどうかチェックし可能ならイベントを起こす
    {
        var character_list = manager.GetCharacterList();
        foreach(var chara in character_list)
        {
            if (chara.EventRunning) continue;

            //少し幅持たせて判定
            var near_top_bottom = Mathf.Abs(chara.transform.position.x - transform.position.x) < 0.1 && Mathf.Abs(chara.transform.position.y - transform.position.y) < 1.1;
            var near_right_left = Mathf.Abs(chara.transform.position.y - transform.position.y) < 0.1 && Mathf.Abs(chara.transform.position.x - transform.position.x) < 1.1;
            if (near_right_left || near_top_bottom)  
            {
                //イベント開始処理
                EventRunning = true;
                Name.enabled = true;
                manager.MessageListner(new Message("event", "start"));
                chara.MessageListner(new Message("event", "start,0"));
                event_opponent = chara;
            }
        }
    } 

    //イベント終了処理
    private void EventEnd(bool online)
    {
        EventRunning = false;
        if(!online)event_opponent.MessageListner(new Message("event","end,0"));
        manager.MessageListner(new Message("event", "end"));
        Name.enabled = false;
    }
        
}