using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    Transform[] childWayStones;
    public List<Transform> childWayStonesList;
    // Start is called before the first frame update
    void Start()
    {
        FillWayStones();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        FillWayStones();

        for (int i = 0; i < childWayStonesList.Count; i++)
        {
            Vector3 pos = childWayStonesList[i].position;
            if (i > 0) {
                Vector3 prev = childWayStonesList[i - 1].position;
                Gizmos.DrawLine(prev, pos);
            }
        }
    }

    public int getCurrentPosition(Transform wayStone)
    {
        return childWayStonesList.IndexOf(wayStone);
    }

    void FillWayStones() {
        childWayStonesList.Clear();
        childWayStones = GetComponentsInChildren<Transform>();
        foreach (Transform child in childWayStones) {
            if (child != this.transform) {
                childWayStonesList.Add(child);
            }
        }
    }

    public void AddWayStone(Transform childWayStone) {
        if (childWayStonesList == null) {
            childWayStonesList = new List<Transform>();
        }
        childWayStonesList.Add(childWayStone);
    }
}
