using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : BaseActor
{
    public BotMovement botMovement;

    private void Start()
    {
        id = 1; //TODO: id of bot is assigned by BotManager
        currentStage = 1;
        base.Start();
    }

    public void PushToStack(Brick brick)
    {
        base.PushToStack(brick);
        botMovement.FindRandomBrick();
    }
}
