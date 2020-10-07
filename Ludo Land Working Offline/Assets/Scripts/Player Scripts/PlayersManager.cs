using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameManager;
using UnityEngine.SceneManagement;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager Instance { set; get; }
    public GameObject mainMenu;
    public GameObject serverMenu;
    public GameObject connectMenu;
    public TextMeshProUGUI playerName;
    //for more extensions
    //public GameObject mainMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    private void Start()
    {
        Instance = this;
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }
    public void OnConnectButton()
    {
        mainMenu.SetActive(false);
        connectMenu.SetActive(true);

    }
    public void OnHostButton()
    {
        try {
            Server s = Instantiate(serverPrefab).GetComponent<Server>();
            s.Init();
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = playerName.text;
            c.isHost = true;
            c.isLocal = true;
            if (c.clientName == "")
            {
                c.clientName = "Host";
            }
            c.ConnectToServer("127.0.0.1", 6312);
        } catch(Exception e) {
            Debug.Log(e.Message);
        }
        mainMenu.SetActive(false);
        serverMenu.SetActive(true);
    }
    public void ConnectToServerButton() {
        string hostAddress = "127.0.0.1";
            //GameObject.Find("HostInput").GetComponent<InputField>().text;
       // if (hostAddress == "") {
         //   hostAddress = "127.0.0.1";
//        }

        try
        {
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = playerName.text;
            if (c.clientName == "") {
                c.clientName = "Client";
            }
            c.isHost = false;
            c.isLocal = false;
            c.ConnectToServer(hostAddress, 6312);
            connectMenu.SetActive(false);
        }
        catch (Exception e) {
            Debug.Log(e.Message);
        }
    }
    public void BackButton() {
        mainMenu.SetActive(true);

        serverMenu.SetActive(false);
        connectMenu.SetActive(false);

        Server s = FindObjectOfType<Server>();
        if (s != null) {
            Destroy(s.gameObject);
        }
        Client c = FindObjectOfType<Client>();
        if (c != null)
        {
            Destroy(c.gameObject);
        }
    }

    public void StartGame(List<GameClient> players) {
        List<Player> playersForGame = new List<Player>();

         foreach (GameClient gc in players)
         {
                Player p = new Player();
                p.playerName =  gc.name;
                p.hasTurn = false;
                p.isLocalPlayer = gc.isLocal;
            if (gc.isLocal) {
                p.playerType = Player.PlayerTypes.HUMAN_LOCAL;
            }
            else {
                p.playerType = Player.PlayerTypes.HUMAN_ONLINE;
            }
            p.hasWon = false;

            playersForGame.Add(p);
         }
        StaticClassForCrossSceneInformation.CrossSceneInformationForPlayerDisplayName = playerName.text;
        StaticClassForCrossSceneInformation.CrossSceneInformationForPlayerList = playersForGame;
        SceneManager.LoadScene("Game");
    }
}
