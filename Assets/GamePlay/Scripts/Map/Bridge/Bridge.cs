using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public Transform targetGround;
    public Collider targetGroundCollider;
    public Transform bridgeBrickContainer;

    internal int maxBrick = 60;

    internal Dictionary<Collider, BridgeBrick> bridgeBrickDict = new Dictionary<Collider, BridgeBrick>();

    internal int currentBrickNum = 0;

    private const float BRICK_DISTANCE_Y = 0.1f;
    private const float BRICK_DISTANCE_Z = 0.5f;

    private ObjectPooling objectPooling;
    private MapManager mapManager;

    void Start()
    {
        objectPooling = ObjectPooling.ins;
        mapManager = MapManager.ins;
    }

    public bool CheckBuildDone()
    {
        if (currentBrickNum >= maxBrick - 1)
        {
            return true;
        }
        return false;
    }

    public void SpawnBridgeBrick(int playerId)
    {
        if (currentBrickNum >= maxBrick - 1) return;

        BridgeBrick bridgeBrick = objectPooling.SpawnFromPool(Constant.BRIDGE_BRICK_TAG).GetComponent<BridgeBrick>();
        bridgeBrick.id = playerId;
        bridgeBrick.meshRenderer.material = mapManager.matLists[playerId];
        bridgeBrick.onBridge = this;

        bridgeBrickDict.Add(bridgeBrick.col, bridgeBrick);

        Transform brickTf = bridgeBrick.transform;
        brickTf.SetParent(bridgeBrickContainer);
        brickTf.localPosition = Vector3.zero;
        brickTf.localPosition = new Vector3(brickTf.localPosition.x, brickTf.localPosition.y + BRICK_DISTANCE_Y * currentBrickNum, brickTf.localPosition.z + BRICK_DISTANCE_Z * currentBrickNum);
        currentBrickNum++;
    }
}
