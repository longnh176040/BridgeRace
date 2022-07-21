using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public Transform trans;
    public Rigidbody rb;
    public BoxCollider bcol;
    public CapsuleCollider scol;
    public MeshRenderer mesh;
    public Material defautMat;

    internal int id;    //Thể hiện layer của brick // = id của player tương ứng
    internal int orderPos; //Lưu vị trí trong mảng các brick
    internal int stage; //Lưu vị trí stage mà nó đang nằm
    internal BaseActor owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constant.PLAYER_TAG) || other.CompareTag(Constant.BOT_TAG))
        {
            if (owner == null)
            {
                owner = Cache.GetActor(other);
            }
            
            owner.PushToStack(this);
            ImmuneBrick();
        }
    }

    public void InitBrick(int id, int order, int stage)
    {
        this.id = id;
        orderPos = order;
        this.stage = stage;
        gameObject.layer = LayerMask.NameToLayer(Constant.PLAYER_LAYER[id]);
        //owner = LevelManager.ins.GetActor(id);
        mesh.material = MapManager.ins.matLists[id];
        bcol.enabled = true;
        scol.enabled = true;
        rb.useGravity = false;
    }

    public void ImmuneBrick()
    {
        bcol.enabled = false;
        scol.enabled = false;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void Reset()
    {
        bcol.enabled = true;
        scol.enabled = true;
        mesh.material = defautMat;
        owner = null;
        rb.useGravity = true;
        rb.isKinematic = false;
        gameObject.layer = Constant.DEFAULT_LAYER;

        if (!BrickSpawner.ins.brickLists[id].Contains(this))
        {
            BrickSpawner.ins.brickLists[id].Remove(this);
        }
    }
}
