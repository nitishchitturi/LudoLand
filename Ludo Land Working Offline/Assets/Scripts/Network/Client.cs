using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    private bool socketReady;
    public bool isLocal;
    public bool isHost;
    private TcpClient socket;
    private NetworkStream networkStream;
    private StreamWriter writer;
    private StreamReader reader;
    public string clientName;
    private List<GameClient> players = new List<GameClient>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void UserConnected(string[] aData, bool host) {
        if (!ClientAlreadyExists(players, aData[1])) {
            Debug.Log("Usr connected");
            GameClient gc = new GameClient();
            gc.name = aData[1];
            gc.isHosting = (aData[2]=="0") ? false:true;
            gc.isLocal = (aData[3]=="0") ? false:true;
            players.Add(gc);
        }
        Debug.Log("players count : "+players.Count);

        if (players.Count == 2) {
            PlayersManager.Instance.StartGame(players);
        }
    }

    private bool ClientAlreadyExists(List<GameClient> players, string name)
    {
        foreach (GameClient player in players)
        {
            if (player.name == name) {
                return true;
            }
        }
        return false;
    }

    public bool ConnectToServer(string host, int port) {
        if (socketReady)
        {
            return false;
        }
        try
        {
            socket = new TcpClient(host, port);
            networkStream = socket.GetStream();
            writer = new StreamWriter(networkStream);
            reader = new StreamReader(networkStream);

            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error : " + e.Message);
        }

        return socketReady;

    }

    //Read messages from server
    private void OnIncomingData(string data) {
        Debug.Log("Client side OnIncomingData : " +data);
        string[] aData = data.Split('|');
        switch (aData[0]) {
            case "SWHO":
                if(aData.Length>1&&aData[1]!=""&& aData[2] != "")
                {
                    UserConnected(aData, true);
                }
                SendData("CWHO|" + clientName + "|" + (isHost ? 1:0).ToString() + "|" + (isLocal ? 1 : 0).ToString());
                break;
            case "SCNN":
                    UserConnected(aData, false);
                break;
        }
    }

    //Send messages to server
    private void SendData(string data) {
        if (!socketReady) {
            return;
        }
        writer.WriteLine(data);
        writer.Flush();

    }

    private void Update()
    {
        if (socketReady) {
            if (networkStream.DataAvailable) {
                string data = reader.ReadLine();
                if (data != null) {
                    OnIncomingData(data);
                }
            }
        }
    }

    private void CloseSocket() {
        if (!socketReady) {
            return;
        }

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }
}

public class GameClient {
    public string name;
    public bool isHosting;
    public bool isLocal;
    //add all that you need for a player
}
