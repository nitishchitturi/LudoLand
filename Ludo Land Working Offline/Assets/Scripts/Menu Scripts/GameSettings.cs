using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public PanelManagerForOfflinePlayer panelManager;
    //----------------------BLUE---------------------
    public void SetBlueHumanType(bool on) {
        if (on) SaveSettings.players[0] = "HUMAN";
    }
    public void SetBlueCPUType(bool on) {
        if (on) SaveSettings.players[0] = "CPU";
    }
    public void SetBlueNoPlayerType()
    {
        panelManager.RemovePlayerPanel(0);
        SaveSettings.players[0] = "NONE";
    }
    //----------------------RED---------------------
    public void SetRedHumanType(bool on)
    {
        if (on) SaveSettings.players[1] = "HUMAN";
    }
    public void SetRedCPUType(bool on)
    {
        if (on) SaveSettings.players[1] = "CPU";
    }
    public void SetRedNoPlayerType()
    {
        panelManager.RemovePlayerPanel(1);
        SaveSettings.players[1] = "NONE";
    }
    //----------------------GREEN---------------------
    public void SetGreenHumanType(bool on)
    {
        if (on) SaveSettings.players[2] = "HUMAN";
    }
    public void SetGreenCPUType(bool on)
    {
        if (on) SaveSettings.players[2] = "CPU";
    }
    public void SetGreenNoPlayerType()
    {
        panelManager.RemovePlayerPanel(2);
        SaveSettings.players[2] = "NONE";
    }
    //----------------------YELLOW---------------------
    public void SetYellowHumanType(bool on)
    {
        if (on) SaveSettings.players[3] = "HUMAN";
    }
    public void SetYellowCPUType(bool on)
    {
        if (on) SaveSettings.players[3] = "CPU";
    }
    public void SetYellowNoPlayerType()
    {
        panelManager.RemovePlayerPanel(3);
        SaveSettings.players[3] = "NONE";
    }
}

public static class SaveSettings {
    //blue, red, green, yellow
    public static string[] players = new string[4];
}