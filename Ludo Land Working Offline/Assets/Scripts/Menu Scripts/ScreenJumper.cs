using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenJumper : MonoBehaviour
{
    public void JumpToScreen(string screenName) {
        SceneManager.LoadScene(screenName);
    }
}
