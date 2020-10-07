using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelManagerForOfflinePlayer : MonoBehaviour
{
    public GameObject playerOnePanel;
    public GameObject playerTwoPanel;
    public GameObject playerThreePanel;
    public GameObject playerFourPanel;

    public GameObject removePlayerOneButton;
    public GameObject removePlayerTwoButton;
    public GameObject removePlayerThreeButton;
    public GameObject removePlayerFourButton;

    public void RemovePlayerPanel(int playerNumber) {
        switch (playerNumber) {
            case 0:
                playerOnePanel.SetActive(false);
                break;
            case 1:
                playerTwoPanel.SetActive(false);
                break;
            case 2:
                playerThreePanel.SetActive(false);
                break;
            case 3:
                playerFourPanel.SetActive(false);
                break;
        }
        if (MinPlayersPanelRemoved()) {
            if (playerOnePanel.activeSelf) {
                removePlayerOneButton.SetActive(false);
            }
            if (playerTwoPanel.activeSelf)
            {
                removePlayerTwoButton.SetActive(false);
            }
            if (playerThreePanel.activeSelf)
            {
                removePlayerThreeButton.SetActive(false);
            }
            if (playerFourPanel.activeSelf)
            {
                removePlayerFourButton.SetActive(false);
            }
        }
    }

    private bool MinPlayersPanelRemoved()
    {
        int count = 0;
        if (!playerOnePanel.activeSelf) {
            count++;
        } if (!playerTwoPanel.activeSelf) {
            count++;
        } if (!playerThreePanel.activeSelf) {
            count++;
        } if (!playerFourPanel.activeSelf) {
            count++;
        }
       
        return count == 2;
    }

    public void GoBackToHomeMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
