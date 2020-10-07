using UnityEngine;
using UnityEngine.Events;

public class DiceRoller : MonoBehaviour
{
    public GameObject definedButton;
    [Header("SELECTOR")]
    public GameObject selector;
    private int activePlayer;
    public UnityEvent OnClick = new UnityEvent();
    Renderer rend;

    // Use this for initialization
    void Start()
    {
        definedButton = this.gameObject;
        OnClick.AddListener(OnClickDiceRoller);
        ChangeColor();
    }

    private void ChangeColor() {

        rend = GetComponent<Renderer>();
        activePlayer = GameManager.instance.activePlayer;
        string currentPlayerColor = GameManager.instance.playerList[activePlayer].color;
        rend.material.color = GetColor(currentPlayerColor);
        if (GameManager.instance.playerList[activePlayer].playerType == GameManager.Player.PlayerTypes.HUMAN_LOCAL) {
            ActivateSelectors(true);
        }
        definedButton.SetActive(true);

    }

    public void ActivateSelectors(bool activate)
    {
        selector.SetActive(activate);
        if (!activate)
        {
            selector.GetComponent<Animator>().enabled = false;
        }
        else {
            selector.GetComponent<Animator>().enabled = true;
        }
    }

    private Color GetColor(string currentPlayerColor)
    {
        switch (currentPlayerColor)
        {
            case "blue":
                return Color.blue;
            case "red":
                return Color.red;
            case "green":
                return Color.green;
            case "yellow":
                return Color.yellow;
        }
        return Color.gray;

    }

    public void OnClickDiceRoller()
    {
        ActivateSelectors(false);

        if (GameManager.instance.playerList[activePlayer].playerType == GameManager.Player.PlayerTypes.HUMAN_LOCAL&&GameManager.instance.diceGameobject.activeSelf)
        {
            GameManager.instance.HumanRolls();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ChangeColor();
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == gameObject)
            {
                OnClick.Invoke();
            }
        }
    }
}

