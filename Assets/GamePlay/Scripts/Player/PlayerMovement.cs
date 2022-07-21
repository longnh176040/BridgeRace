using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public BaseActor actor;
    public Rigidbody rb;
    public Transform trans;
    public Transform transRotate;
    public float speed;

    public DynamicJoystick joystick;

    public Transform stepCheck;
    public float stepHeight = 0.5f;
    public float stepOffset = 2f;
    private Transform lastHitGround;
    private float limitMinZPos = float.MinValue;
    private float limitMaxZPos;



    [SerializeField]
    private bool canMove = false;

    void Update()
    {
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
        CheckSpawnBridge();
        ClimbBridge();
    }


    private void CheckSpawnBridge()
    {
        RaycastHit hit;
        if (Physics.Raycast(stepCheck.position, Vector3.down, out hit, 2f))
        {
            if (hit.transform.CompareTag(Constant.BRIDGE_TAG))
            {
                Bridge b = Cache.GetBridge(hit.collider);

                if (actor.GetStackNum() > 0 && !b.CheckBuildDone())
                {
                    b.SpawnBridgeBrick(actor.id);
                    actor.PopFromStack();
                }
                else if (b.CheckBuildDone())
                {
                    if (!b.isBuilt)
                    {
                        float targetGroundSize = b.targetGround.GetComponent<Collider>().bounds.size.z / 2;
                        //limitMaxZPos = b.targetGround.position.z + targetGroundSize;
                        limitMaxZPos = float.MaxValue;
                        limitMinZPos = b.targetGround.position.z - targetGroundSize + 0.25f;

                        b.isBuilt = true;
                        BrickSpawner.ins.DequeueUnuseBrick(actor.id, actor.currentStage);
                        actor.currentStage++;
                        BrickSpawner.ins.InitMapWithId(actor.id, actor.currentStage);
                    }
                }
            }
        }
    }

    private void ClimbBridge()
    {
        RaycastHit hit;
        if (Physics.Raycast(stepCheck.position, trans.TransformDirection(Vector3.down), out hit, 2f))
        {
            if (hit.transform.CompareTag(Constant.BRIDGE_BRICK_TAG))
            {
                lastHitGround = hit.transform;
                limitMaxZPos = hit.transform.position.z + hit.collider.bounds.size.z / 2;
                trans.position += new Vector3(0f, stepOffset * Time.fixedDeltaTime, 0f);
                Vector3 clampedPos = new Vector3(trans.position.x, 
                    Mathf.Clamp(trans.position.y, hit.transform.position.y-0.5f, hit.transform.position.y),
                    Mathf.Clamp(trans.position.z, limitMinZPos, limitMaxZPos));
                trans.position = clampedPos;
                //Debug.Log("1");
            }
            else
            {
                if (lastHitGround == null) return;
                Vector3 clampedPos = new Vector3(trans.position.x, 
                    Mathf.Clamp(trans.position.y, lastHitGround.position.y - 0.5f, lastHitGround.position.y),
                    Mathf.Clamp(trans.position.z, limitMinZPos, limitMaxZPos));
                trans.position = clampedPos;
                //Debug.Log("2");
            }
        }
        else
        {
            if (lastHitGround == null) return;
            Vector3 clampedPos = new Vector3(trans.position.x, 
                Mathf.Clamp(trans.position.y, lastHitGround.position.y - 0.5f, lastHitGround.position.y),
                Mathf.Clamp(trans.position.z, limitMinZPos, limitMaxZPos));
            trans.position = clampedPos;
            //Debug.Log("3");
        }
    }
}
