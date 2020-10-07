using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [System.Serializable]
    public class Player {
        public string playerName;
        public PlayerCoin[] playerCoins;
        public bool isLocalPlayer;
        public bool hasTurn;
        public string color;
        public enum PlayerTypes
        {
            HUMAN_LOCAL,
            HUMAN_ONLINE,
            CPU,
            NO_PLAYER
        }
        public PlayerTypes playerType;
        public bool hasWon;
        public List<PlayerCoin> playerCoinsWon;
        public string playerDisplayName;
    }
    public List<Player> playerList = new List<Player>();
    //STATE_MACHINE
    public enum States {
        WAITING,
        ROLL_DICE,
        SWITCH_PLAYER,
    }
    public States state;
    public int activePlayer;
    bool switchPlayer;
    private bool turnPossible = true;
    //HUMAN_INPUTS
    public GameObject diceGameobject;
    public List<WayStone> protectedWayStones;
    public List<WayStone> protectedWayStonesOutside;
    [HideInInspector]public int humanRolledDice;
    //GAME_OBJECTS for our buttons
    public Dice dice;
    public DiceRoller diceRoller;
    [HideInInspector] public string[] winners;
    public GameObject blueBaseCrown; 
    public GameObject redBaseCrown; 
    public GameObject greenBaseCrown;
    public GameObject yellowBaseCrown; 
    public GameObject blueBaseFirst; 
    public GameObject redBaseFirst; 
    public GameObject greenBaseFirst; 
    public GameObject yellowBaseFirst; 
    public GameObject blueBaseSecond; 
    public GameObject redBaseSecond; 
    public GameObject greenBaseSecond; 
    public GameObject yellowBaseSecond; 
   
    void Start()
    {
        diceRoller.selector.SetActive(false);

        winners = new string[playerList.Count];
        for (int i = 0; i < winners.Length; i++)
        {
            winners[i] = string.Empty;
        }
        int firstPlayer = Random.Range(0, playerList.Count);
        activePlayer = firstPlayer;

    }

    void CPUDice() {
        diceRoller.selector.SetActive(false);
        diceRoller.definedButton.SetActive(true);
        dice.RollDice();
    }
    void Awake()
    {
        instance = this;
        if (winners == null) {
            winners = new string[playerList.Count];
            for (int i = 0; i < winners.Length; i++)
            {
                winners[i] = string.Empty;
            }
        }
        int indexCount = playerList.Count;
        for (int i = 0; i < indexCount; i++)
        {
            if (SaveSettings.players[i] == "HUMAN")
            {
                playerList[i].playerType = Player.PlayerTypes.HUMAN_LOCAL;
            }
            else if (SaveSettings.players[i] == "CPU")
            {
                playerList[i].playerType = Player.PlayerTypes.CPU;
            }
            else {
                playerList[i].playerType = Player.PlayerTypes.NO_PLAYER;
                RemovePlayer(i);
            }

        }
        RemovePlayersPermanentlyFromBoard();
    }

    private void RemovePlayersPermanentlyFromBoard()
    {
        List<Player> tempPlayerList = new List<Player>();
        foreach (Player player in playerList)
        {
            tempPlayerList.Add(player);
        }
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].playerType.Equals(Player.PlayerTypes.NO_PLAYER)) {
                tempPlayerList.Remove(playerList[i]);
                continue;
            }
        }

        if (playerList.Count == tempPlayerList.Count) {
            return;
        }
        else {

            playerList = new List<Player>();
            foreach (Player player in tempPlayerList)
            {
                playerList.Add(player);
            }
        }
      

    }

    private void RemovePlayer(int playerNumber)
    {
        for (int i = 0; i < playerList[playerNumber].playerCoins.Length; i++)
        {
            playerList[playerNumber].playerCoins[i].coinPrefab.SetActive(false);
        }

    }

    void Update()
    {
        if (playerList[activePlayer].playerType == Player.PlayerTypes.CPU)
        {
            switch (state)
            {
                case States.ROLL_DICE:
                    {
                        if (turnPossible) {
                            StartCoroutine(RollDiceDelay());
                            state = States.WAITING;
                        }
                    }
                    break;
                case States.SWITCH_PLAYER:
                    {
                        if (turnPossible)
                        {
                            StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                    }
                    break;
                case States.WAITING:
                    {
                    }
                    break;
                
            }
        }
        if (playerList[activePlayer].playerType == Player.PlayerTypes.HUMAN_LOCAL)
        {
            switch (state)
            {
                case States.ROLL_DICE:
                    {
                        if (turnPossible)
                        {
                            //Deactivate highlights
                            ActivateDiceButton(true);

                            state = States.WAITING;
                        }
                    }
                    break;
                case States.SWITCH_PLAYER:
                    {
                        if (turnPossible)
                        {
                            //Deactivate highlights
                            StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                    }
                    break;
                case States.WAITING:
                    {
                    }
                    break;

            }
        }
    }
    //-----------------------CPU GAME CODE-----------------------------------------
    #region cpu-game-code
    public void RollDice(int diceValue) {
        int diceNumber = diceValue;
        if (playerList[activePlayer].playerType == Player.PlayerTypes.CPU)
        {
            if (diceNumber == 6)
            {
                CheckStartWayStone(diceNumber);
            }

            if (diceNumber < 6)
            {
                MoveAPlayerCoin(diceNumber);
            }
        }
        else if (playerList[activePlayer].playerType == Player.PlayerTypes.HUMAN_LOCAL)
        {
            humanRolledDice = diceValue;
            HumanRollDice();

        }


    }
    IEnumerator RollDiceDelay() {
        yield return new WaitForSeconds(1);
        CPUDice();
    }
    void CheckStartWayStone(int diceNumber) {
        
        for (int i = 0; i < playerList[activePlayer].playerCoins.Length; i++)
            {
                PlayerCoin playerCoin = playerList[activePlayer].playerCoins[i];

                if (!playerCoin.IsPlayerCoinOut())
                {
                    playerCoin.LeaveBase();
                    state = States.WAITING;
                    return;
                }
            }
            MoveAPlayerCoin(diceNumber);
    }

    void MoveAPlayerCoin(int diceNumber) {
        List<PlayerCoin> movableCoins = new List<PlayerCoin>();
        List<PlayerCoin> movableCoinsHavingKickChance = new List<PlayerCoin>();
        //Fill possible moves to list
        for (int i = 0; i < playerList[activePlayer].playerCoins.Length; i++)
        {
            PlayerCoin playerCoin = playerList[activePlayer].playerCoins[i];

            if (playerCoin.IsPlayerCoinOut()) {
                //check for possible kick
                if (playerCoin.CheckPossibleKick(
                    playerCoin.playerCoinID, diceNumber)) {
                    
                    movableCoinsHavingKickChance.Add(playerCoin);
                    continue;
                }
                //check for movable coins if no kicks available
                if (playerCoin.CheckPossibleMove(
                     diceNumber))
                {
                    movableCoins.Add(playerCoin);
                }
            }
        }

        //Perform kick if possible
        if (movableCoinsHavingKickChance.Count > 0) {
            int randomChoice = Random.Range(0, movableCoinsHavingKickChance.Count);
            movableCoinsHavingKickChance[randomChoice].StartTheMove(diceNumber);
            state = States.WAITING;
            return;
        }
        //Perform move if possible
        if (movableCoins.Count > 0)
        {
            int priorityNumber = GetFarthestCoinNumber(movableCoins);
            movableCoins[priorityNumber].StartTheMove(diceNumber);
            state = States.WAITING;
            return;
        }
        //No action possible
        //Switch player
        state = States.SWITCH_PLAYER;

    }

    private int GetFarthestCoinNumber(List<PlayerCoin> movableCoins)
    {
        int priority = 0;
        for (int i = 0; i < movableCoins.Count; i++)
        {
           if(movableCoins[i].currentWayStone != movableCoins[i].startWayStone)
           {
                if (!movableCoins[i].currentWayStone.isProtectedWaystone)
                {
                    priority = i;
                }
               
            }
        }
        
        if (priority == 0) {
            priority = Random.Range(0, movableCoins.Count);
        }
        return priority;
    }

    IEnumerator SwitchPlayer(){
        if(switchPlayer){
            yield break;
        }
        switchPlayer = true;
        yield return new WaitForSeconds(1);
        //SET NEXT PLAYER
        SetNextActivePlayer();
        switchPlayer = false;
    }

    void SetNextActivePlayer() {
       ShowCountTextForCurrentPlayer(false);

        activePlayer++;
        activePlayer %= playerList.Count;

        int available = 0;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (!playerList[i].hasWon) {
                available++;
            }
        }
        if (playerList[activePlayer].hasWon && available > 1)
        {
            SetNextActivePlayer();
            return;
        }
        else if (available < 2) {
            state = States.WAITING;
            return;
        }
        ShowCountTextForCurrentPlayer(true);
         state = States.ROLL_DICE;
    }

    private void ShowCountTextForCurrentPlayer(bool on)
    {
        string playerColor = playerList[activePlayer].color;
        for (int i = 0; i < protectedWayStones.Count; i++) {
            switch (playerColor)
            {
                case "blue":
                    GameObject[] blueCounts = GameObject.FindGameObjectsWithTag("BlueCount");
                    foreach (GameObject blueCount in blueCounts)
                    {
                        if (blueCount)
                        {
                            if (on && blueCount.GetComponent<TextMesh>()!=null)
                            {
                                blueCount.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            }
                            else
                            {
                                if (blueCount.GetComponent<TextMesh>() != null)
                                {
                                    blueCount.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                                }
                                else
                                {
                                    blueCount.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
                                }

                            }
                        }
                    }
                   
                    break;
                case "red":
                    GameObject[] redCounts = GameObject.FindGameObjectsWithTag("RedCount");
                    foreach (GameObject redCount in redCounts) {
                        if (redCount)
                        {
                            if (on && redCount.GetComponent<TextMesh>() != null)
                            {
                                redCount.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            }
                            else
                            {
                                if (redCount.GetComponent<TextMesh>() != null)
                                {
                                    redCount.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                                }
                                else
                                {
                                    redCount.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
                                }

                            }
                        }
                    }
                       
                    break;
                case "green":
                    GameObject[] greenCounts = GameObject.FindGameObjectsWithTag("GreenCount");
                    foreach (GameObject greenCount in greenCounts)
                    {
                        if (greenCount)
                        {
                            if (on && greenCount.GetComponent<TextMesh>() != null)
                            {
                                greenCount.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            }
                            else
                            {
                                if (greenCount.GetComponent<TextMesh>() != null)
                                {
                                    greenCount.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                                }
                                else
                                {
                                    greenCount.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
                                }

                            }
                        }
                    }
                    break;
                case "yellow":
                    GameObject[] yellowCounts = GameObject.FindGameObjectsWithTag("YellowCount");
                    foreach (GameObject yellowCount in yellowCounts)
                    {
                        if (yellowCount)
                        {
                            if (on && yellowCount.GetComponent<TextMesh>() != null)
                            {
                                yellowCount.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            }
                            else
                            {
                                if (yellowCount.GetComponent<TextMesh>() != null)
                                {
                                    yellowCount.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                                }
                                else {
                                    yellowCount.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
                                }

                            }
                        }
                    }
                    break;
            }
        }
           
    }


    public void ReportIfTurnPossible(bool possible) {
        turnPossible = possible;
    }

    public void ReportWinning() {
      

        playerList[activePlayer].hasWon = true;
        int winNumber = 0;
        
        for (int i = 0; i < winners.Length; i++)
        {
            if (winners[i] == "") {
                winners[i] = playerList[activePlayer].playerName;
                winNumber = i;
                ActivateCrownAndWinPlace(playerList[activePlayer].color, i);
                break;
            }
           
        }
        //This piece of code is to add the last player to the winners list
        if(CheckIfGameOver()) {
            for (int i = 0; i < playerList.Count; i++)
            {
                if (!playerList[i].hasWon) {
                    winners[playerList.Count-1] = playerList[i].playerName;
                }
            }
            SceneManager.LoadScene("GameResults");
        }
    }

    private void ActivateCrownAndWinPlace(string color, int i)
    {
        if (i == playerList.Count - 1) {
            return;
        }
        switch (color) {
            case "blue":
                blueBaseCrown.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                if (i == 0)
                {
                    blueBaseFirst.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                else if(i == 1)
                {
                    blueBaseSecond.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                break;
            case "red":
                redBaseCrown.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                if (i == 0)
                {
                    redBaseFirst.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                else if (i == 1)
                {
                    redBaseSecond.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                break;
            case "green":
                greenBaseCrown.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                if (i == 0)
                {
                    greenBaseFirst.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                else if (i == 1)
                {
                    greenBaseSecond.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                break;
            case "yellow":
                yellowBaseCrown.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                if (i == 0)
                {
                    yellowBaseFirst.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                else if (i == 1)
                {
                    yellowBaseSecond.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                break;
        }
    }

    private bool CheckIfGameOver()
    {
        int count = 0;
        for (int i = 0; i < winners.Length; i++)
        {
            if (winners[i] != "") {
                count++;
            }
        }
        return (count == playerList.Count-1);
    }
    #endregion
   
    //-----------------------HUMAN INPUT CODE-----------------------------------------
    #region human-input
    void ActivateDiceButton(bool isHumanPlaying) {
        if (PreviousMoveDone()) {
            diceGameobject.SetActive(isHumanPlaying);
            diceRoller.definedButton.SetActive(isHumanPlaying);
        }
    }

    private bool PreviousMoveDone()
    {
       PlayerCoin[] currPlayerCoins = playerList[activePlayer].playerCoins;
        foreach (PlayerCoin playerCoin in currPlayerCoins)
        {
            if (playerCoin.selector.activeSelf) {
                return false;
            }
        }

        return true;
    }

    public void DeactivateAllSelectors() {
        for (int i = 0; i < playerList.Count; i++)
        {
            for (int j = 0; j < playerList[i].playerCoins.Length; j++)
            {
                playerList[i].playerCoins[j].ActivateSelectorsAndTurn(false);
            }
        }
    }

    public void HumanRolls() {
        diceRoller.ActivateSelectors(false);
        dice.RollDice();
        diceRoller.definedButton.SetActive(false);
    }

    //This sits on the roll dice button
    public void HumanRollDice()
    {
    
        //Gather list of movable coins
        List<PlayerCoin> movableCoins = new List<PlayerCoin>();
        //Fill possible moves to list
        //dice < 6 - actiavte selectors for coins outside base and keep has turn true
        if (humanRolledDice < 6)
        {
            movableCoins.AddRange(ReturnPossiblePlayerCoins());
        }
        //dice == 6 - actiavte selectors for all coins and keep has turn true for all
        if (humanRolledDice == 6)
        {
            for (int i = 0; i < playerList[activePlayer].playerCoins.Length; i++)
            {
                PlayerCoin playerCoin = playerList[activePlayer].playerCoins[i];
                if (!playerCoin.IsPlayerCoinOut()) {
                    movableCoins.Add(playerCoin);
                }else if (!playerCoin.reachedEnd && !CheckIfReachedVictoryPath(playerCoin) && !SameCoinInWay(playerCoin)) {
                    movableCoins.Add(playerCoin);
                }
            }
            state = States.ROLL_DICE;
        }
        
        //activating selctors and has turn true NOW!
        if (movableCoins.Count > 0)
        {
            for (int i = 0; i < movableCoins.Count; i++)
            {
                movableCoins[i].ActivateSelectorsAndTurn(true);
            }
        }
        else{
            state = States.SWITCH_PLAYER;
        }
        
    }

    private bool SameCoinInWay(PlayerCoin playerCoinLocal)
    {
        bool result = false;

        int tempPos = playerCoinLocal.routePosition + 6;
        if (playerCoinLocal.fullPath[tempPos].isTaken && !playerCoinLocal.fullPath[tempPos].isProtectedWaystone) {
            if (playerCoinLocal.fullPath[tempPos].playerCoin.playerCoinID == playerCoinLocal.playerCoinID) {
                result = true;
            }
        }
        return result;
    }

    private bool CheckIfReachedVictoryPath(PlayerCoin playerCoin)
    {
        bool result = false;
        if (playerCoin.currentWayStone.isProtectedWaystone && !protectedWayStonesOutside.Contains(playerCoin.currentWayStone)) {
            result = true;
        }

        return result;
    }

    List<PlayerCoin> ReturnPossiblePlayerCoins() {
        List<PlayerCoin> movableCoins = new List<PlayerCoin>();

        for (int i = 0; i < playerList[activePlayer].playerCoins.Length; i++)
        {
            PlayerCoin playerCoin = playerList[activePlayer].playerCoins[i];

            if (playerCoin.IsPlayerCoinOut())
            {
                //check for possible kick
                if (playerCoin.CheckPossibleKick(
                   playerCoin.playerCoinID, humanRolledDice))
                {

                    movableCoins.Add(playerCoin);
                    continue;
                }
                //check for movable coins if no kicks available
                if (playerCoin.CheckPossibleMove(
                     humanRolledDice))
                {
                    movableCoins.Add(playerCoin);
                }
            }
        }
        return movableCoins;
    }
    #endregion

    public static class StaticClassForCrossSceneInformation
    {
        public static List<Player> CrossSceneInformationForPlayerList { get; set; }
        public static string CrossSceneInformationForPlayerDisplayName { get; set; }
    }
}
