using System.Collections.Generic;
using UnityEngine;

public class WayStone : MonoBehaviour
{
    public bool isTaken;
    public PlayerCoin playerCoin;
    public GameObject blueCount;
    public GameObject redCount;
    public GameObject greenCount;
    public GameObject yellowCount;
    private TextMesh blueTextMesh;
    private TextMesh redTextMesh;
    private TextMesh greenTextMesh;
    private TextMesh yellowTextMesh;

    public WayStone bluePosWayStone;
    public WayStone redPosWayStone;
    public WayStone greenPosWayStone;
    public WayStone yellowPosWayStone;

    public List<PlayerCoin> playerCoinsReachedEnd = new List<PlayerCoin>();
    public List<PlayerCoin> playerCoinsProtected = new List<PlayerCoin>();
    public bool isProtectedWaystone;
    public Vector3 originalPosition;
    public bool isHomeStone;

    private WayStone() {
    }

    public WayStone getInstance() {
        return this;
    }

    public List<PlayerCoin> getPlayerCoinsReachedEnd() {
        return playerCoinsReachedEnd;
    }


    public List<PlayerCoin> getPlayerCoinsProtected()
    {
        return playerCoinsProtected;
    }


   


    private void ShowFloatingCountText(int playerCoinIDColor)
    {
        List<PlayerCoin> blueCoins = new List<PlayerCoin>();
        List<PlayerCoin> redCoins = new List<PlayerCoin>();
        List<PlayerCoin> greenCoins = new List<PlayerCoin>();
        List<PlayerCoin> yellowCoins = new List<PlayerCoin>();

        for (int i = 0; i < playerCoinsProtected.Count; i++)
        {

            int playerCoinId = playerCoinsProtected[i].playerCoinID;

            switch (playerCoinId) {
                case 1:
                    blueCoins.Add(playerCoinsProtected[i]);
                    break;
                case 2:
                    redCoins.Add(playerCoinsProtected[i]);
                    break;
                case 3:
                    greenCoins.Add(playerCoinsProtected[i]);
                    break;
                case 4:
                    yellowCoins.Add(playerCoinsProtected[i]);
                    break;
            }
             
          }

        Vector3 rot = Quaternion.identity.eulerAngles;
        rot = new Vector3(rot.x, rot.y - 45, rot.z);
        switch (playerCoinIDColor) {
            case 1:
                SetUpText(blueCoins, rot, 1);
                break;
            case 2:
                SetUpText(redCoins, rot, 2);
                break;
            case 3:
                SetUpText(greenCoins, rot, 3);
                break;
            case 4:
                SetUpText(yellowCoins, rot, 4);
                break;
        }
       

     }

   private void SetUpText(List<PlayerCoin> coins, Vector3 rot, int coinId)
    {
        if (coins.Count <= 1) {
            DestroyFloatingText(coinId);
            return;
        }
        string text = ""+ coins.Count;

        Vector3 textPos = gameObject.transform.position + new Vector3(0.0f, 2f, 0.0f);

        switch (coinId)
            {
                case 1:
                    if (!blueTextMesh)
                    {
                         if (blueCount)
                            {
                                blueCount.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            }

                    GameObject curCountBlue = null;

                    if (bluePosWayStone)
                    {
                        textPos = bluePosWayStone.gameObject.transform.position;
                        curCountBlue = Instantiate(blueCount, textPos, bluePosWayStone.gameObject.transform.rotation, bluePosWayStone.gameObject.transform);

                    }
                    else
                    {
                        curCountBlue = Instantiate(blueCount, textPos, Quaternion.Euler(rot), transform);
                    }
                    curCountBlue.transform.Rotate(30f, 20f, 0f);
                        blueTextMesh = curCountBlue.GetComponent<TextMesh>();
                       
                    }

                blueTextMesh.text = text;

                    break;
                case 2:
                    if (!redTextMesh)
                    {
                        if (redCount)
                        {
                            redCount.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        }
                    GameObject curCountRed = null;

                    if (redPosWayStone)
                    {
                        textPos = redPosWayStone.gameObject.transform.position;
                        curCountRed = Instantiate(redCount, textPos, redPosWayStone.gameObject.transform.rotation, redPosWayStone.gameObject.transform);
                    }
                    else {
                        curCountRed = Instantiate(redCount, textPos, Quaternion.Euler(rot), transform);
                    }

                    curCountRed.transform.Rotate(30f, 20f, 0f);
                    redTextMesh = curCountRed.GetComponent<TextMesh>();
                       
                    }

                redTextMesh.text = text;

                    break;
                case 3:
                    if (!greenTextMesh)
                    {
                        if (greenCount)
                        {
                            greenCount.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        }
                    GameObject curCountGreen = null;

                    if (greenPosWayStone)
                    {
                        textPos = greenPosWayStone.gameObject.transform.position;
                        curCountGreen = Instantiate(greenCount, textPos, greenPosWayStone.gameObject.transform.rotation, greenPosWayStone.gameObject.transform);
                    }
                    else
                    {
                        curCountGreen = Instantiate(greenCount, textPos, Quaternion.Euler(rot), transform);
                    }

                    curCountGreen.transform.Rotate(30f, 20f, 0f);
                    greenTextMesh = curCountGreen.GetComponent<TextMesh>();
                       

                    }

                greenTextMesh.text = text;

                    break;
                case 4:
                    if (!yellowTextMesh)
                        {
                            if (yellowCount)
                            {
                                yellowCount.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            }
                    GameObject curCountYellow = null;

                    if (yellowPosWayStone)
                    {
                        textPos = yellowPosWayStone.gameObject.transform.position;
                        curCountYellow = Instantiate(yellowCount, textPos, yellowPosWayStone.gameObject.transform.rotation, yellowPosWayStone.gameObject.transform);
                    }
                    else
                    {
                        curCountYellow = Instantiate(yellowCount, textPos, Quaternion.Euler(rot), transform);
                    }
                    curCountYellow.transform.Rotate(30f, 20f, 0f);
                    yellowTextMesh = curCountYellow.GetComponent<TextMesh>();
                            
                        }
               
                yellowTextMesh.text = text;

                    break;
            }
               
    }

    public void InitiateCountText(int playerCoinID)
    {
            ShowFloatingCountText(playerCoinID);
    }

    public void DestroyFloatingText(int coinId)
    {
        switch (coinId)
        {
            case 1:
                if(blueTextMesh)
                {
                    Destroy(blueTextMesh, 1.0f);
                }
                DestroyCountBoard("BlueCount");
                break;
            case 2:
                if (redTextMesh)
                {
                    Destroy(redTextMesh, 1.0f);
                }
                DestroyCountBoard("RedCount");
                break;
            case 3:
                if (greenTextMesh)
                { 
                    Destroy(greenTextMesh, 1.0f);
                }
                DestroyCountBoard("GreenCount");
                break;
            case 4:
                if (yellowTextMesh)
                {
                   Destroy(yellowTextMesh, 1.0f);
                }
                DestroyCountBoard("YellowCount");
                break;
        }

    }

    private void DestroyCountBoard(string count)
    {
        GameObject[] countBoards = GameObject.FindGameObjectsWithTag(count);
        foreach (GameObject countBoard in countBoards)
        {
            countBoard.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);

        }
    }
}
