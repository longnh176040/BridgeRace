using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeBrick : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Collider col;
    
    [SerializeField]
    internal int id; // = id của player tương ứng
    internal Bridge onBridge;

    public void ChangeOwner(int id)
    {
        this.id = id;
        meshRenderer.material = MapManager.ins.matLists[id];
    }
}
