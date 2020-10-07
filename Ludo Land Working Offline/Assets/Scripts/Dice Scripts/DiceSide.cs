using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSide : MonoBehaviour
{
    bool onGround;
    public int oppositeSideDiceValue;

    void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("Ground")) {
            onGround = true;
        } 
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    public bool OnGround() {
        return onGround;
    }
}
