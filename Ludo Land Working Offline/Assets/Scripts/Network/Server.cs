using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour
{
    public int port = 6312;
    private List<ServerClient> clientList;
    private List<ServerClient> disconnectedClientList;

    private TcpListener server;
    private bool serverStarted;

    public void Init() {
        DontDestroyOnLoad(gameObject);
        clientList = new List<ServerClient>();
        disconnectedClientList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
        }
        catch (Exception e) {
            Debug.Log("Socket error : " + e.Message);
        }
    }

    private void Update()
    {
        if (!serverStarted) {
            return;
        }
        foreach (ServerClient client in clientList)
        {
            if (!IsConnected(client.tcpClient))
            {
                client.tcpClient.Close();
                disconnectedClientList.Add(client);
                continue;
            }
            else {
                NetworkStream ns = client.tcpClient.GetStream();
                if (ns.DataAvailable) {
                    StreamReader reader = new StreamReader(ns, true);
                    string data = reader.ReadLine();

                    if (data != null) {
                        OnIncomingData(client, data);
                        }
                }


            }
        }

        for (int i = 0; i < disconnectedClientList.Count - 1; i++)
        {
            //show player disconnected based on this info
            clientList.Remove(disconnectedClientList[i]);
            disconnectedClientList.RemoveAt(i);
        }
    }

    // server read data
    private void OnIncomingData(ServerClient client, string data)
    {
        Debug.Log("Server side OnIncomingData : " + data);

        string[] aData = data.Split('|');
        switch (aData[0])
        {
            case "CWHO":
                client.clientName = aData[1];
                client.isHost = (aData[2] == "0") ? false : true;
                client.isLocal = (aData[3] == "0") ? false : true;
                BroadCastData("SCNN|" + client.clientName + "|" + (client.isHost ? 1 : 0).ToString() + "|" + (client.isLocal ? 1 : 0).ToString(), clientList);
                break;
        }
    }

    // server write data
    private void BroadCastData(string data, ServerClient sc)
    {
        List<ServerClient> sclist = new List<ServerClient> { sc };
        BroadCastData(data, sclist);
    }

    // server write data

    private void BroadCastData(string data, List<ServerClient> scList)
        {

            foreach (ServerClient client in scList)
            {
            try
            {
                StreamWriter writer = new StreamWriter(client.tcpClient.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch(Exception e) {
                Debug.Log("Write error : " + e.Message);
            }
        }
    }
    private bool IsConnected(TcpClient client)
    {
        try {
            if (client != null && client.Client != null && client.Client.Connected)
            {
                if (client.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else {
                return false;
            }
        } catch {
            return false;
        }
    }

    private void StartListening() {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private void AcceptTcpClient(IAsyncResult ar) {
        TcpListener listener = (TcpListener)ar.AsyncState;
        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
        clientList.Add(sc);
        StartListening();
        string allUsrs = "";
        foreach (ServerClient scl in clientList)
        {
            allUsrs += scl.clientName + "|";
        }
        BroadCastData("SWHO|" + allUsrs , clientList[clientList.Count-1]);
        Debug.Log("someone is connected to the server");
    }
}

public class ServerClient {
    public string clientName;
    public TcpClient tcpClient;
    public bool isHost;
    public bool isLocal;
    public ServerClient(TcpClient tcpClient) {
        this.tcpClient = tcpClient;
    }
}
