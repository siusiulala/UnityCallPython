using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SocketServer : MonoBehaviour {


    Socket socketListen;//用於監聽的socket
    Socket socketConnect;//用於通訊的socket
    string RemoteEndPoint;     //客戶端的網路節點  
    Dictionary<string, Socket> dicClient = new Dictionary<string, Socket>();//連線的客戶端集合

    public class SocketEventArgs : EventArgs
    {
        public byte[] data { get; set; }
        public SocketEventArgs(byte[] _data)
        {
            data = _data;
        }
    }
    public EventHandler<SocketEventArgs> ReceiveData;

    void Start () {
        DontDestroyOnLoad(this.gameObject);
        StartSocket();

    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnDestroy()
    {
        foreach(Socket socket in dicClient.Values)
        {
            if(socket != null)
                socket.Close();
        }
        if (socketConnect != null)
            socketConnect.Close();
        if (socketListen != null)
            socketListen.Close();
    }

    void StartSocket()
    {
        //建立套接字
        IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 2020);
        socketListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //繫結埠和IP
        socketListen.Bind(ipe);
        //設定監聽
        socketListen.Listen(10);
        //print(socketListen.ReceiveBufferSize);
        //連線客戶端
        AsyncConnect(socketListen);
    }

    /// <summary>
    /// 連線到客戶端
    /// </summary>
    /// <param name="socket"></param>
    void AsyncConnect(Socket socket)
    {
        try
        {
            socket.BeginAccept(asyncResult =>
            {
                //獲取客戶端套接字
                socketConnect = socket.EndAccept(asyncResult);
                RemoteEndPoint = socketConnect.RemoteEndPoint.ToString();
                dicClient.Add(RemoteEndPoint, socketConnect);
                AsyncReceive(socketConnect);
                AsyncConnect(socketListen);
            }, null);


        }
        catch (Exception ex)
        {

        }

    }

    /// <summary>
    /// 接收訊息
    /// </summary>
    /// <param name="client"></param>
    void AsyncReceive(Socket socket)
    {
        byte[] data = new byte[1024*1024];
        try
        {
            //開始接收訊息
            socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
            asyncResult =>
            {
                try
                {
                    int length = socket.EndReceive(asyncResult);
                    if (length > 0)
                    {
                        print(length);
                        //string msg = Encoding.UTF8.GetString(data);
                        //print(msg);

                        this.ReceiveData(this, new SocketEventArgs(data));
                    }
                }
                    //setText(Encoding.UTF8.GetString(data));

                    catch (Exception)
                {
                    AsyncReceive(socket);
                }

                AsyncReceive(socket);
            }, null);

        }
        catch (Exception ex)
        {
        }
    }
}
