using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotMovement : MonoBehaviour
{
    public BaseActor actor;
    public Transform trans;
    public Transform transRotate;
    public NavMeshAgent agent;

    internal Bot_State botState;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

public enum Bot_State
{
    Collect, 
    OnBridge
}
