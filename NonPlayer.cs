using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NonPlayer : Character
{ 
    public new void Start()
    {
        base.Start();
        EventRunning = false;
        Name.enabled = false;
        IPAdress = "*";
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
                case "event":
                    if (option.Contains("start"))
                    {
                        EventRunning = true;
                        Name.enabled = true;
                        //ローカルでイベント開始なら全オンラインプレイヤーへ通知
                        if (option == "start,0") { manager.MessageListner(new Message("send", "*>event:start,1")); }
                    }
                    else
                    {
                        EventEnd();
                        //ローカルでイベント終了なら全オンラインプレイヤーへ通知
                        if (option == "end,0") { manager.MessageListner(new Message("send", "*>event:end,1")); }
                    }
                    break;
            }
        }
    }

}