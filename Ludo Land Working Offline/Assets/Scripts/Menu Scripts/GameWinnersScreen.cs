using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameWinnersScreen : MonoBehaviour
{
    public Text first, second, third, fourth;

    // Start is called before the first frame update
    void Start()
    {
       
        first.text += "  " + GameManager.instance.winners[0];
        second.text += "  " + GameManager.instance.winners[1];
        if (GameManager.instance.winners.Length > 2)
        {
            third.text += "  " + GameManager.instance.winners[2];
        }
        else {
            third.enabled = false;
        }
        if (GameManager.instance.winners.Length > 3)
        {
            fourth.text += "  " + GameManager.instance.winners[3];
        }
        else
        {
            fourth.enabled = false;
        }
    }

    public void GoBackToHomeMenu(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
