using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Character : MessageObject
{
    public enum Direction { Up, Down, Right, Left }  //方向

    public MessageManager manager;

    public bool EventRunning { get; set; }  //イベントの最中かどうか
    public TextMeshPro Name;  //名前（今回はplayer、non_player、IPアドレスのどれか）
    public string IPAdress { get; set; }  //IPアドレス（プレイヤーはループバック,npcは*）

    //初期化
    public new void Start()
    {
        base.Start();
        EventRunning = false;
        Name.enabled = false;
    }

    //移動
    public void Move(Direction dir)
    {
        var position = this.transform.position;
        var rigidbody = gameObject.GetComponent<Rigidbody2D>();
        switch (dir)
        {
            case Direction.Up:
                rigidbody.MovePosition(new Vector2(position.x, position.y + 1));
                break;
            case Direction.Down:
                rigidbody.MovePosition(new Vector2(position.x, position.y - 1));
                break;
            case Direction.Right:
                rigidbody.MovePosition(new Vector2(position.x + 1, position.y));
                break;
            case Direction.Left:
                rigidbody.MovePosition(new Vector2(position.x - 1, position.y));
                break;
        }
    }

    //イベント終了
    protected void EventEnd()
    {
        EventRunning = false;
        Name.enabled = false;
    }
}