using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Message
{
    /// <summary>
    /// メッセージクラス
    /// コマンドと引数的な感じで定義
    /// </summary>
    
    public string Order { get; set; }
    public string Option { get; set; }

    public Message(string order)
    {
        this.Order = order;
    }

    public Message(string order,string option)
    {
        this.Order = order;
        this.Option = option;
    }
}

