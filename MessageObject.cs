using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageObject : MonoBehaviour
{
    /// <summary>
    /// メッセージを扱うオブジェクトの基底クラス
    /// メッセージはキューで管理
    /// </summary>

    protected Queue<Message> message_queue;

    public void Start()
    {
        message_queue = new Queue<Message>();
    }

    public void Update()
    {
        if (message_queue.Count != 0)
        {
            ProcessingMessage();
        }
    }

    public virtual void ProcessingMessage()  //メッセージ処理
    {
        Debug.Log("Unimplemented");
    }

    public void MessageListner(Message mes)　　//メッセージ受信
    {
        message_queue.Enqueue(mes);
    }

}

