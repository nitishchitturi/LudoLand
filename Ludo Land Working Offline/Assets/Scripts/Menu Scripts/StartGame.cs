using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public Dropdown themeSelection = null;
    public string sceneName = "LudoGame";
    // for initiating a game scene
    void Start()
    {
        for (int i = 0; i < SaveSettings.players.Length; i++)
        {
            SaveSettings.players[i] = "CPU";
        }
    }

    public void SetTheme() {
       int themeChoice = themeSelection.value;
        if (themeChoice == 1) {
            sceneName = "AnimalLudoGame";
        }
    }

    public void StartTheGame() {

        SceneManager.LoadScene(sceneName);
    }
}
