using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoManager : MonoBehaviour
{
    public static InfoManager info;
    [SerializeField] TextMeshProUGUI infoDisplay;
    
    void Awake()
    {
        info = this;
        infoDisplay.text = "";
    }

    public void ShowMessage(string message)
    {
        infoDisplay.text = message;


    }
}
