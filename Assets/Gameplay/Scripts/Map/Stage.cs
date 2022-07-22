using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public int id;
    public Transform[] endGates;
    public Transform[] bridge;

    internal bool[] isGatesChosen; //List check xem gate nào đã đc chọn

    void Start()
    {
        MapManager.ins.SetupStageDictionary(this);

        isGatesChosen = new bool[endGates.Length];
    }
}
