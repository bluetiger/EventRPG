using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnlinePlayer : Character
{
    public override void ProcessingMessage()
    {
        while (message_queue.Count != 0)
        {
            var mes = message_queue.Dequeue();
            switch (mes.Order)
            {
                case "move":
                    if (mes.Option == "1") Move(Direction.Up);
                    else if (mes.Option == "2") Move(Direction.Down);
                    else if (mes.Option == "3") Move(Direction.Right);
                    else if (mes.Option == "4") Move(Direction.Left);
                    break;
                case "event":
                    if (mes.Option.Contains("start"))
                    {
                        EventRunning = true;
                        Name.enabled = true;
                        //ローカルでイベント開始なら全オンラインプレイヤーへ通知
                        if (mes.Option == "start,0") { manager.MessageListner(new Message("send", IPAdress + ">event:start,2")); }
                    }
                    else
                    {

                        EventEnd();
                        //ローカルでイベント終了なら全オンラインプレイヤーへ通知
                        if (mes.Option == "end,0") { manager.MessageListner(new Message("send", IPAdress + ">event:end,2")); }
                    }
                    break;
            }
        }
    }

}