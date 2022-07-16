using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constant
{
    public const string PLAYER_TAG = "Player";
    public const string BOT_TAG = "Bot";
    public const string BRICK_TAG = "Brick";
    public const string BRIDGE_TAG = "Bridge";
    public const string BRIDGE_BRICK_TAG = "Bridge Brick";
    public const string GROUND_TAG = "Ground";

    public const int DEFAULT_LAYER = 0;
    public static string[] PLAYER_LAYER = new string[6] { "Player1", "Player2", "Player3", "Player4", "Player5", "Player6", };

    public const int TOTAL_BRICK_PER_PLAYER = 100;
    public const float RESPAWN_BRICK_TIME = 3f;

}
