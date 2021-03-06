using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constant
{
    public const string ANIM_ACTION = "Action";
    public const int IDLE_ANIM = 0;
    public const int RUN_ANIM = 1;
    public const int FALL_ANIM = 2;
    public const int WIN_ANIM = 3;

    public const string PLAYER_TAG = "Player";
    public const string BOT_TAG = "Bot";
    public const string BRICK_TAG = "Brick";
    public const string BRIDGE_TAG = "Bridge";
    public const string BRIDGE_BRICK_TAG = "Bridge Brick";
    public const string GROUND_TAG = "Ground";
    public const string FINISH_TAG = "Finish";

    public const int STAGE_NUM = 3;
    public const int DEFAULT_LAYER = 0;
    public static string[] PLAYER_LAYER = new string[6] { "Player1", "Player2", "Player3", "Player4", "Player5", "Player6", };

    public const int TOTAL_BRICK_PER_PLAYER = 250;
    public const float RESPAWN_BRICK_TIME = 3f;

}
