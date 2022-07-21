using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : BaseActor
{
    private void Start()
    {
        base.Start();
        id = 1; //TODO: id of bot is assigned by BotManager
        currentStage = 1;
    }
}
