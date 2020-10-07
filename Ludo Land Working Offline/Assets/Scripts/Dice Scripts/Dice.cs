using UnityEngine;

public class Dice : MonoBehaviour
{
    Rigidbody rigidBody;
    bool isLanded;
    bool thrown;
    Vector3 initialPosition;
    public DiceSide[] diceSides;
    public int diceValue;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.diceGameobject.gameObject.SetActive(true);
        initialPosition = transform.position;

        rigidBody = GetComponent<Rigidbody>();

        rigidBody.useGravity = false;
    }

    public void RollDice()
    {
        GameManager.instance.diceGameobject.gameObject.SetActive(true);

        Reset();

        if (!thrown && !isLanded)
        {
            thrown = true;

            rigidBody.useGravity = true;

            rigidBody.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));

        }
        else if (thrown && isLanded) {
            Reset();
        }
    }

    void Reset()
    {
        transform.position = initialPosition;
        rigidBody.isKinematic = false;
        thrown = false;
        isLanded = false;
        rigidBody.useGravity = false;


    }

    void Update()
    {
        if (rigidBody.IsSleeping() && !isLanded && thrown)
        {
            isLanded = true;
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            //checked dice value here
            SideValueCheck();
        }
        else if (rigidBody.IsSleeping() && isLanded && diceValue == 0)
        {
            //ROLL AGAIN
            RollAgain();
        }
    }

    void RollAgain() {
        Reset();
        thrown = true;
        rigidBody.useGravity = true;
        rigidBody.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
    }

    void SideValueCheck() {
        diceValue = 0;
        foreach (DiceSide side in diceSides)
        {
            if (side.OnGround()) {
                diceValue = side.oppositeSideDiceValue;
               
                //Report to gamemanager
                GameManager.instance.RollDice(diceValue);
            }
        }
       
    }
}
