using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


class Udp
{
    private UdpClient udp;
    
    public const int buf_size = 1024;
    public byte[] buf = new byte[buf_size];
    public event EventHandler ReciveMessage;
    public string recive_ip;

    public Udp()
    {
        System.Net.IPEndPoint localEP = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 1221);
        udp = new System.Net.Sockets.UdpClient(localEP);
        udp.BeginReceive(ReceiveCallback, udp);
    }
    
    public void Close()
    {
        udp.Close();
    }
    
    private void ReceiveCallback(IAsyncResult asyncResult)
    {
        var udp = asyncResult.AsyncState as UdpClient;

        IPEndPoint remoteEP = null;
        try
        {
            buf = udp.EndReceive(asyncResult, ref remoteEP);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("{0}:{1}",ex.Message, ex.ErrorCode);
            return;
        }
        catch (ObjectDisposedException ex)
        {
            return;
        }

        if (buf.Length > 0)
        {
            recive_ip = remoteEP.Address.ToString();
            ReciveMessage(this, EventArgs.Empty);
            //メッセージ処理が終了するまで受信を開始しない
        }
    }

    public void WaitMessage()
    {
        udp.BeginReceive(ReceiveCallback, udp);
    }

    public void SendMessage(string ip ,string mes)
    {
        var bytes = Encoding.UTF8.GetBytes(mes);
        udp.BeginSend(bytes, bytes.Length, new IPEndPoint(IPAddress.Parse(ip), 1221),SendCallback, udp);
    }

    private void SendCallback(IAsyncResult ar)
    {
       UdpClient udp = (UdpClient)ar.AsyncState;
        try
        {
            udp.EndSend(ar);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("{0}:{1}",ex.Message, ex.ErrorCode);
        }
    }
}

