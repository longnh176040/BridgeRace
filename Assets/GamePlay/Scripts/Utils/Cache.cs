using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Cache
{
    private static Dictionary<Collider, BaseActor> colActorDict = new Dictionary<Collider, BaseActor>();
    private static Dictionary<Collider, Bridge> colBridgerDict = new Dictionary<Collider, Bridge>();

    public static BaseActor GetActor(Collider col)
    {
        if (!colActorDict.ContainsKey(col))
        {
            BaseActor actor = col.GetComponent<BaseActor>();
            colActorDict.Add(col, actor);
        }

        return colActorDict[col];
    }

    public static Bridge GetBridge(Collider col)
    {
        if (!colBridgerDict.ContainsKey(col))
        {
            Bridge bridge = col.GetComponent<Bridge>();
            colBridgerDict.Add(col, bridge);
        }

        return colBridgerDict[col];
    }
}
