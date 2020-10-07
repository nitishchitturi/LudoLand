using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    public Text welcomePlayerNote = null;
    private void Start()
    {
       string playerName = PlayerPrefs.GetString("PlayerName");
        if(playerName!=null || playerName != "")
        {
            welcomePlayerNote.text = "Welcome " + playerName + " !!!";
        }
    }

    public void HostLobby() {
        networkManager.StartHost();
        landingPagePanel.SetActive(false);
    }

    public void JumpToScreen(string screenName) {
        SceneManager.LoadScene(screenName);
    }
}
