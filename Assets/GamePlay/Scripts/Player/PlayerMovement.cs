using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController actor;
    public Rigidbody rb;
    public Transform trans;
    public Transform transRotate;
    public float speed;
    internal bool canMove = false;

    public DynamicJoystick joystick;

    public LayerMask layerMask;
    public Transform stepCheck;
    //public float stepHeight = 0.5f;
    public float stepOffset = 2f;
    private Transform lastHitGround;
    private float limitMinZPos = float.MinValue;
    private float limitMaxZPos;
    private Bridge curBridge;


    void Update()
    {
        if (!canMove) return;

        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            actor.SetAnim(Constant.RUN_ANIM);
            rb.velocity = new Vector3(joystick.Horizontal * speed, trans.position.y, joystick.Vertical * speed);
            transRotate.localRotation = Quaternion.LookRotation(rb.velocity);
            transRotate.localEulerAngles = new Vector3(0, transRotate.localEulerAngles.y, 0);
        }
        else
        {
            actor.SetAnim(Constant.IDLE_ANIM);
            rb.velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        CheckSpawnBridge();
        ClimbBridge();
    }


    private void CheckSpawnBridge()
    {
        if (actor.isWin) return;

        RaycastHit hit;
        if (Physics.Raycast(stepCheck.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.CompareTag(Constant.BRIDGE_TAG))
            {
                curBridge = Cache.GetBridge(hit.collider);

                if (actor.GetStackNum() > 0 && !curBridge.CheckBuildDone())
                {
                    curBridge.SpawnBridgeBrick(actor.id);
                    actor.PopFromStack();
                }
                else if (curBridge.CheckBuildDone())
                {
                    if (!actor.hasFinishedStage[actor.currentStage-1])
                    {
                        actor.hasFinishedStage[actor.currentStage-1] = true;
                        float targetGroundSize = curBridge.targetGroundCollider.bounds.size.z / 2;
                        limitMaxZPos = float.MaxValue;
                        limitMinZPos = curBridge.targetGround.position.z - targetGroundSize + 0.25f;

                        BrickSpawner.ins.DequeueUnuseBrick(actor.id, actor.currentStage);
                        actor.currentStage++;

                        if (actor.currentStage > Constant.STAGE_NUM)
                        {
                            actor.isWin = true;
                            if (!LevelManager.ins.HaveWin)
                            {
                                actor.SetWin(0);
                            }
                            return;
                        }

                        BrickSpawner.ins.InitMapWithId(actor.id, actor.currentStage);
                    }
                }
            }
        }
    }

    private void ClimbBridge()
    {
        RaycastHit hit;
        if (Physics.Raycast(stepCheck.position, trans.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.CompareTag(Constant.BRIDGE_BRICK_TAG))
            {
                //CheckOwner
                curBridge = hit.transform.GetComponent<BridgeBrick>().onBridge;
                BridgeBrick bridgeBrick = curBridge.bridgeBrickDict[hit.collider];

                if (actor.GetStackNum() > 0)
                {
                    if (bridgeBrick.id != actor.id)
                    {
                        bridgeBrick.ChangeOwner(actor.id);
                        actor.PopFromStack();
                    }
                    limitMaxZPos = hit.transform.position.z + hit.collider.bounds.size.z / 2;
                }

                //Clamp Position
                lastHitGround = hit.transform;
                
                trans.position += new Vector3(0f, stepOffset * Time.fixedDeltaTime, 0f);
                Vector3 clampedPos = new Vector3(trans.position.x,
                    Mathf.Clamp(trans.position.y, hit.transform.position.y - 0.5f, hit.transform.position.y),
                    Mathf.Clamp(trans.position.z, limitMinZPos, limitMaxZPos));
                trans.position = clampedPos;
            }

            else if (hit.transform.CompareTag(Constant.GROUND_TAG))
            {
                if (!actor.isWin)
                {
                    if (actor.GetStackNum() > 1)
                    {
                        limitMaxZPos = float.MaxValue;
                    }
                    else
                    {
                        limitMaxZPos = hit.transform.position.z + hit.collider.bounds.size.z / 2 - 1f;
                    }
                    limitMinZPos = hit.transform.position.z - hit.collider.bounds.size.z / 2 + 0.25f;
                }

                float avaiPosY = hit.transform.position.y + hit.collider.bounds.size.y/2;

                Vector3 clampedPos = new Vector3(trans.position.x, avaiPosY, 
                    Mathf.Clamp(trans.position.z, limitMinZPos, limitMaxZPos));
                trans.position = clampedPos;
            }
            else
            {
                if (lastHitGround == null) return;
                Vector3 clampedPos = new Vector3(trans.position.x,
                    Mathf.Clamp(trans.position.y, lastHitGround.position.y - 1f, lastHitGround.position.y),
                    Mathf.Clamp(trans.position.z, limitMinZPos, limitMaxZPos));
                trans.position = clampedPos;
            }
        }
        else
        {
            if (lastHitGround == null) return;
            Vector3 clampedPos = new Vector3(trans.position.x, 
                Mathf.Clamp(trans.position.y, lastHitGround.position.y - 1f, lastHitGround.position.y),
                Mathf.Clamp(trans.position.z, limitMinZPos, limitMaxZPos));
            trans.position = clampedPos;
        }
    }
}
