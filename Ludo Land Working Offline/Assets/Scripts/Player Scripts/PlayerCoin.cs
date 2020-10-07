using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoin : MonoBehaviour
{
    public int playerCoinID;

    public GameObject coinPrefab;
    public WayStone victoryWayStone;
    [Header("ROUTES")]
    private Route commonPath;//outer route
    public Route victoryPath;//inner route
    public Route fullCommonPath;//common outer route for every coin
    private bool kickedAnother;
    public List<WayStone> fullPath;

    [Header("WAYSTONES")]
    public WayStone startWayStone;//starting waystone for this player
    public WayStone baseWayStone;//waystone(STAND) in the player base

    public WayStone currentWayStone;
    public WayStone goalWayStone;

    public int routePosition;
    int startPathIndex;
    private Vector3 scaleChange;
    private Vector3 positionChange;
    [Header("BOOLS")]
    private bool isOut;
    bool isMoving;
    bool hasTurn;//for human input
    public bool reachedEnd;
    [Header("SELECTOR")]
    public GameObject selector;
    private int steps;//dice value
    private int doneSteps;
    //ARC MOVEMENT
    float amplitude = 1.5f;
    float time = 0f;
    void Start()
    {
        //lets do it from here not from unity - do what -> Setup the waystones that this coin will travel before going to its victory path
        Transform startPathTransform = startWayStone.gameObject.transform;
        FillCommonPath(startPathTransform);
        startPathIndex = commonPath.getCurrentPosition(startPathTransform);
        createFullPath();
        ActivateSelectorsAndTurn(false);
    
    }
    
    bool MoveInArcToNextWayStone(Vector3 startPos, Vector3 goalPos, float speed) {
        time += speed * Time.deltaTime;
        Vector3 myPos = Vector3.Lerp(startPos, goalPos, time);
        myPos.y += amplitude * Mathf.Sin(Mathf.Clamp01(time) * Mathf.PI);

        float[] changePosAngles = GetAnglesForRotationBasedOnMovement();

        //go back to original rotation
        var originalRot = gameObject.transform.localRotation.eulerAngles; //get the angles
        originalRot.Set(270f, 0f, 0f); //set the angles
        gameObject.transform.localRotation = Quaternion.Euler(originalRot); //update the transform

        //Now set rotation that we want
        var rot = gameObject.transform.localRotation.eulerAngles; //get the angles
        rot.Set(changePosAngles[0], changePosAngles[1], changePosAngles[2]); //set the angles
        gameObject.transform.localRotation = Quaternion.Euler(rot); //update the transform
        return goalPos != (transform.position = Vector3.Lerp(transform.position, myPos, time));

    }

    private float[] GetAnglesForRotationBasedOnMovement()
    {
       
        float[] angles = new float[3];

        //Default case
        angles[0] = 270f;
        angles[1] = 0f;
        angles[2] = 0f;

        List<int> northNodes = new List<int> { 0, 1, 2, 3, 4, 10, 11, 18, 19, 20, 21, 22, 50, 51, 52, 53, 54, 55};
        List<int> eastNodes = new List<int> {12, 13, 14, 15, 16, 17, 23, 24, 31, 32, 34, 35};
        List<int> westNodes = new List<int> { 5, 6, 7, 8, 9, 38, 39, 40, 41, 42, 43, 49};
        List<int> southNodes = new List<int> {25, 26, 27, 28, 29, 30, 36, 37, 44, 45, 46, 47, 48};

        if (playerCoinID == 2 || playerCoinID == 4)
        {
            northNodes.Add(56);
        }
        else {
            southNodes.Add(56);
        }

        int wayStoneId = fullPath.IndexOf(goalWayStone);
        //0, 90, 270 for blue to turn like red
        //0, 270, 90 for blue to turn like yellow
        //90, 0, 180 for blue to turn like green
        //to stay like blue set back original angles
        /*
            N
        W   C   E
            S
        */
        if (westNodes.Contains(wayStoneId))
        {
            angles[0] = 0f;
            angles[1] = 270f;
            angles[2] = 90f;
        }
        else if (northNodes.Contains(wayStoneId))
        {
            angles[0] = 270f;
            angles[1] = 0f;
            angles[2] = 0f;
        }
        else if (eastNodes.Contains(wayStoneId))
        {
            angles[0] = 0f;
            angles[1] = 90f;
            angles[2] = 270f;
        }
        else if (southNodes.Contains(wayStoneId))
        {
            angles[0] = 90f;
            angles[1] = 0f;
            angles[2] = 180f;
        }

        return angles;
    }

    bool MoveInArcToNextWayStone(Vector3 startPos, Vector3 goalPos, float speed, PlayerCoin coin)
    {
        time += speed * Time.deltaTime;
        Vector3 myPos = Vector3.Lerp(startPos, goalPos, time);
        myPos.y += amplitude * Mathf.Sin(Mathf.Clamp01(time) * Mathf.PI);
        return goalPos != (coin.transform.position = Vector3.Lerp(coin.transform.position, myPos, time));

    }


    private void FillCommonPath(Transform startPathTransform)
    {
        //initiate the common route for this coin
        commonPath = new Route();
        //Every coin has 50 common waystones to cross before walking the victory path
       int counter = 50;
       int initialIndex = fullCommonPath.getCurrentPosition(startPathTransform);

        int length = 0;
        if (initialIndex == 0) {
            length = fullCommonPath.childWayStonesList.Count - 1;
        }
        else {
            length = fullCommonPath.childWayStonesList.Count;
        }
        for (int i = initialIndex; i < length; i++)
        {
            commonPath.AddWayStone(fullCommonPath.childWayStonesList[i].GetComponent<WayStone>().transform);
            counter--;
        }
        if (counter > 0) {
            for (int j = 0; j < length && counter>=0; j++)
            { 
                commonPath.AddWayStone(fullCommonPath.childWayStonesList[j].GetComponent<WayStone>().transform);
                counter--;
            }
        }
    }

    void createFullPath()
    {
       
        for (int i = 0; i < commonPath.childWayStonesList.Count; i++)
        {
            int tempPos = startPathIndex + i;
            tempPos %= commonPath.childWayStonesList.Count;
            fullPath.Add(commonPath.childWayStonesList[tempPos].GetComponent<WayStone>());
        }
       
        for (int i = 0; i < victoryPath.childWayStonesList.Count; i++)
        {
           
            fullPath.Add(victoryPath.childWayStonesList[i].GetComponent<WayStone>());
        }

        fullPath.Add(this.victoryWayStone.GetComponent<WayStone>());
       
    }
   
    IEnumerator Move(int diceNumber){
        kickedAnother = false;
        int newPlayerCoinsCount = 0;
        if (isMoving) {
            yield break;
        }
        isMoving = true;
        if (currentWayStone.isProtectedWaystone)
        {
            int counter = currentWayStone.getPlayerCoinsProtected().Count;

            currentWayStone.getPlayerCoinsProtected().Remove(this);
            counter = currentWayStone.getPlayerCoinsProtected().Count;
            newPlayerCoinsCount = counter;
            
            newPlayerCoinsCount = currentWayStone.getPlayerCoinsProtected().Count;
            if (newPlayerCoinsCount != 0 )
            {
                changeCurrentWayStoneText(newPlayerCoinsCount);
            }
            if (CheckIfSameColorStonesWhenMovingAway(currentWayStone))
            {
                for (int i = 0; i < currentWayStone.playerCoinsProtected.Count; i++)
                {
                    if (this.name.Equals(currentWayStone.playerCoinsProtected[i].name)) {
                        continue;
                    }
                    Vector3 changeOfPos = currentWayStone.gameObject.transform.position;
                    currentWayStone.playerCoinsProtected[i].gameObject.transform.position = changeOfPos;
                }
            }

        }
        while (steps > 0) {
            routePosition++;
            Vector3 nextPos = fullPath[routePosition].gameObject.transform.position;
           
            Vector3 startPos = fullPath[routePosition - 1].gameObject.transform.position;
            goalWayStone = fullPath[routePosition];

            //This change should happen only if this is the final step for the coin and is landing on protected stone
            if (goalWayStone.isProtectedWaystone && !goalWayStone.isHomeStone && steps == 1 && !CheckIfSameColorStonesWhenMovingTo(goalWayStone, playerCoinID)) {
                switch (playerCoinID)
                {
                    case 1:
                        //new Vector3 is x, z, y
                        WayStone fakeWayStoneBlue = GetFakeWayStone(goalWayStone, playerCoinID);
                        nextPos = fakeWayStoneBlue.gameObject.transform.position;
                        break;
                    case 2:
                        //new Vector3 is x, z, y
                        WayStone fakeWayStoneRed = GetFakeWayStone(goalWayStone, playerCoinID);
                        nextPos = fakeWayStoneRed.gameObject.transform.position;
                        break;
                    case 3:
                        //new Vector3 is x, z, y
                        WayStone fakeWayStoneGreen = GetFakeWayStone(goalWayStone, playerCoinID);
                        nextPos = fakeWayStoneGreen.gameObject.transform.position;
                        break;
                    case 4:
                        //new Vector3 is x, z, y
                        WayStone fakeWayStoneYellow = GetFakeWayStone(goalWayStone, playerCoinID);
                        nextPos = fakeWayStoneYellow.gameObject.transform.position;
                        break;
                }
                RearrangeAllCoinsExceptTheOneChanged(goalWayStone, this);
            }

            while (MoveInArcToNextWayStone(startPos, nextPos, 8f))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            time = 0;
            steps--;
            doneSteps++;
        }
        PlayerCoin opponent = null;
        //check for possible kicks
        if (goalWayStone == this.victoryWayStone) {
            GameManager.instance.playerList[GameManager.instance.activePlayer].playerCoinsWon.Add(this);
            reachedEnd = true;
            //for an extra chance if coin reached end only if there is atleast one coin that has not reached end
            if(GameManager.instance.playerList[GameManager.instance.activePlayer].playerCoinsWon.Count < 4)
            {
                kickedAnother = true;
            }
        }

        if (goalWayStone.isTaken && !goalWayStone.isProtectedWaystone)
        {
            //Kick the stone if it is others
            opponent = goalWayStone.playerCoin;
            opponent.ReturnToBase();

            kickedAnother = true;
        } else if (goalWayStone.isTaken && goalWayStone.isProtectedWaystone) {
            goalWayStone.getPlayerCoinsProtected().Add(this);
            int count = goalWayStone.getPlayerCoinsProtected().Count;

            ChangeGoalWayStoneText(count);
        }
      
        currentWayStone.playerCoin = null;
        if(currentWayStone.getPlayerCoinsProtected().Count==0)
        {
            currentWayStone.isTaken = false;
        }

        goalWayStone.playerCoin = this;
        goalWayStone.isTaken = true;
        
       currentWayStone = goalWayStone;
        if (currentWayStone.isProtectedWaystone && !currentWayStone.getPlayerCoinsProtected().Contains(this)) {
            currentWayStone.getPlayerCoinsProtected().Add(this);
        }
        
        goalWayStone = null;
        //Report to the game manager
        //WIN CONDITION CHECK
        if (WinCondition()) {
            GameManager.instance.ReportWinning();
        }
        //Switch the player
        if (diceNumber == 6 || kickedAnother)
        {
            //same player rolls dice again becoz he rolled a 6 or if he kicked an oppponent's stone
            GameManager.instance.state = GameManager.States.ROLL_DICE;
        }
        else {
            //switch player if nothing more doable by this player
            GameManager.instance.state = GameManager.States.SWITCH_PLAYER;
        }
        
        isMoving = false;

    }

    //IEnumerator RearrangeAllCoinsExceptTheOneChanged(WayStone goalWayStone, PlayerCoin coin)
    private void RearrangeAllCoinsExceptTheOneChanged(WayStone wayStone, PlayerCoin coin)
    {
        for (int i = 0; i < wayStone.playerCoinsProtected.Count; i++)
        {
            if (coin.name.Equals(wayStone.playerCoinsProtected[i].name)) {
                continue;
            }
            Vector3 pos = wayStone.gameObject.transform.position;
            WayStone fakeWayStone = GetFakeWayStone(wayStone, wayStone.playerCoinsProtected[i].playerCoinID);
            Vector3 endPos = fakeWayStone.gameObject.transform.position;

            wayStone.playerCoinsProtected[i].gameObject.transform.position = endPos;
        }
    }

    private WayStone GetFakeWayStone(WayStone goalWayStone, int playerCoinID)
    {
        WayStone waystone = null;
        switch (playerCoinID)
        {
            case 1:
                //new Vector3 is x, z, y
                waystone = goalWayStone.bluePosWayStone;
                break;
            case 2:
                //new Vector3 is x, z, y
                waystone = goalWayStone.redPosWayStone;
                break;
            case 3:
                //new Vector3 is x, z, y
                waystone = goalWayStone.greenPosWayStone;
                break;
            case 4:
                //new Vector3 is x, z, y
                waystone = goalWayStone.yellowPosWayStone;
                break;
        }
        return waystone;
    }

    private bool CheckIfSameColorStonesWhenMovingAway(WayStone waystone)
    {
         
        bool result = true;
        int playerCoinNumber = -1;
        for (int i = 0; i < waystone.playerCoinsProtected.Count; i++)
        {

            if (this.name.Equals(waystone.playerCoinsProtected[i].name))
            {
                continue;
            }
            if (playerCoinNumber == -1) {
                playerCoinNumber = waystone.playerCoinsProtected[i].playerCoinID;
            }
            if (waystone.playerCoinsProtected[i].playerCoinID == playerCoinNumber)
            {
                result = true;
            }
            else {
                result = false;
                break;
            }
        }
        return result;
    }

    private bool CheckIfSameColorStonesWhenMovingTo(WayStone waystone, int playerCoinNumber)
    {
        bool result = true;
        for (int i = 0; i < waystone.playerCoinsProtected.Count; i++)
        {

            if (this.name.Equals(waystone.playerCoinsProtected[i].name))
            {
                continue;
            }
           
            if (waystone.playerCoinsProtected[i].playerCoinID == playerCoinNumber)
            {
                result = true;
            }
            else
            {
                result = false;
                break;
            }
        }
        return result;
    }

    private void ChangeGoalWayStoneText(int countUpdate)
    {
        goalWayStone.InitiateCountText(playerCoinID);
     }

    private void changeCurrentWayStoneText(int countUpdate)
    {
        currentWayStone.InitiateCountText(playerCoinID);
     }
        
       

    bool MoveToNextNode(Vector3 goalPos, float speed) {
        return goalPos != (transform.position = Vector3.MoveTowards
            (transform.position, 
            goalPos,
            speed * Time.deltaTime));
    }

    public bool IsPlayerCoinOut() {
        return isOut;
    }

    public void LeaveBase() {
        steps = 1;
        isOut = true;
        routePosition = 0;
        StartCoroutine(MoveOut());
    }

    IEnumerator MoveOut()
    {
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;
        while (steps > 0)
        {
            WayStone goalWayStoneLocal = fullPath[routePosition];
            Vector3 nextPos = fullPath[routePosition].gameObject.transform.position;

            if (!CheckIfSameColorStonesWhenMovingTo(goalWayStoneLocal, playerCoinID))
            {
                switch (playerCoinID)
                {
                    case 1:
                        //new Vector3 is x, z, y
                        WayStone fakeWayStoneBlue = GetFakeWayStone(goalWayStoneLocal, playerCoinID);
                        nextPos = fakeWayStoneBlue.gameObject.transform.position;
                        break;
                    case 2:
                        //new Vector3 is x, z, y
                        WayStone fakeWayStoneRed = GetFakeWayStone(goalWayStoneLocal, playerCoinID);
                        nextPos = fakeWayStoneRed.gameObject.transform.position;
                        break;
                    case 3:
                        //new Vector3 is x, z, y
                        WayStone fakeWayStoneGreen = GetFakeWayStone(goalWayStoneLocal, playerCoinID);
                        nextPos = fakeWayStoneGreen.gameObject.transform.position;
                        break;
                    case 4:
                        //new Vector3 is x, z, y
                        WayStone fakeWayStoneYellow = GetFakeWayStone(goalWayStoneLocal, playerCoinID);
                        nextPos = fakeWayStoneYellow.gameObject.transform.position;
                        break;

                }
                
                RearrangeAllCoinsExceptTheOneChanged(goalWayStoneLocal, this);
            }
            Vector3 basePos = baseWayStone.gameObject.transform.position;
            //TODO: Remove this code - This code was before arc movement was introduced
            //- KEEPING HERE FOR OTHER PREFABS THAT NEED STRAIGHT MOVEMENT LIKE CARS
            // while (MoveToNextNode(nextPos, 8f))
            //  {
            //      yield return null;
            //  }
            while (MoveInArcToNextWayStone(basePos, nextPos, 1f)) {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            time = 0;
            steps--;
            doneSteps++;
        }
        //Update position of the coin
        goalWayStone = fullPath[routePosition];
       
        goalWayStone.getPlayerCoinsProtected().Add(this);
        int countOfCoinsProt = goalWayStone.getPlayerCoinsProtected().Count;

        if (goalWayStone.isTaken)
        {
            int count = goalWayStone.getPlayerCoinsProtected().Count;

            ChangeGoalWayStoneText(countOfCoinsProt);
        }

        goalWayStone.playerCoin = this;
        goalWayStone.isTaken = true;
        currentWayStone = goalWayStone;
        
        goalWayStone = null;

        //Report this event to the game Manager
        
        GameManager.instance.state = GameManager.States.ROLL_DICE;
        isMoving = false;
    }

    public bool CheckPossibleMove(int diceNumber) {

        if (reachedEnd) {
            return false;
        }
        int tempPos = routePosition + diceNumber;
        
        if (tempPos >= fullPath.Count) {
            return false;
        }
        
        return !fullPath[tempPos].isTaken;
    }

    public bool CheckPossibleKick(int playerCoinId, int diceNumber) {

        int tempPos = routePosition + diceNumber;
        if (tempPos >= fullPath.Count)
        {
            return false;
        }
        if (fullPath[tempPos].isTaken) {
         
            if (!fullPath[tempPos].isProtectedWaystone && playerCoinId == fullPath[tempPos].playerCoin.playerCoinID) {
                return false;
            }
            return true;
        }
     
        return false;
    }

    public void StartTheMove(int diceNumber)
    {
        steps = diceNumber;
        StartCoroutine(Move(diceNumber));
    }

    public void ReturnToBase() {
        StartCoroutine(ReturnHome());
    }

    IEnumerator ReturnHome() {
        Vector3 currentWayStonePosition = currentWayStone.gameObject.transform.position;

        GameManager.instance.ReportIfTurnPossible(false);
        routePosition = 0;
        currentWayStone = null;
        goalWayStone = null;
        isOut = false;
        doneSteps = 0;

        Vector3 baseWayStonePosition = baseWayStone.gameObject.transform.position;
        
        while (MoveInArcToNextWayStone(currentWayStonePosition, baseWayStonePosition, 2f))
        {
            yield return null;
        }
        GameManager.instance.ReportIfTurnPossible(true);
    }


    bool WinCondition()
    {
        if (GameManager.instance.playerList[GameManager.instance.activePlayer].playerCoinsWon.Count == 4) {
            return true;
        }
        return false;
    }

    //--------------------------Human Input--------------------
    #region humaninput
    public void ActivateSelectorsAndTurn(bool activate)
    {
        selector.SetActive(activate);
        //To make sure only coins with selectors activated are movable coins
        hasTurn = activate;
    }

    void OnMouseDown() {
        if (hasTurn) {
            if (!isOut)
            {
                LeaveBase();
            }
            else {
                StartTheMove(GameManager.instance.humanRolledDice);
            }
            GameManager.instance.DeactivateAllSelectors();
        }
        
    }
    #endregion
}
