using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public BaseActor actor;
    public Animator anim;
    public Rigidbody rb;
    public Transform trans;
    public Transform transRotate;
    public float speed;

    public DynamicJoystick joystick;

    public Transform stepCheck;
    public float stepHeight = 0.5f;
    public float stepOffset = 2f;
    private Transform lastHitGround;
    private float limitZPos;

    private int animState;
    private string ANIM_ACTION = "Action";
    private const int IDLE_ANIM = 0;
    private const int RUN_ANIM = 1;

    [SerializeField]
    private bool canMove = false;

    void Update()
    {
        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            SetAnim(RUN_ANIM);
            rb.velocity = new Vector3(joystick.Horizontal * speed, trans.position.y, joystick.Vertical * speed);
            transRotate.localRotation = Quaternion.LookRotation(rb.velocity);
            transRotate.localEulerAngles = new Vector3(0, transRotate.localEulerAngles.y, 0);
        }
        else
        {
            SetAnim(IDLE_ANIM);
            rb.velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        CheckSpawnBridge();
        ClimbBridge();
    }

    private void SetAnim(int state)
    {
        if (animState != state)
        {
            animState = state;
            anim.SetInteger(ANIM_ACTION, animState);
        }
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
                    limitZPos = b.targetGround.position.z + b.targetGround.GetComponent<Collider>().bounds.size.z / 2;
                }
            }
        }
    }

    private void ClimbBridge()
    {
        RaycastHit hit;
        if (Physics.Raycast(stepCheck.position, trans.TransformDirection(Vector3.down), out hit, 1f))
        {
            if (hit.transform.CompareTag(Constant.BRIDGE_BRICK_TAG))
            {
                lastHitGround = hit.transform;
                limitZPos = hit.transform.position.z + hit.collider.bounds.size.z / 2;
                trans.position += new Vector3(0f, stepOffset * Time.fixedDeltaTime, 0f);
                Vector3 clampedPos = new Vector3(trans.position.x, 
                    Mathf.Clamp(trans.position.y, hit.transform.position.y-0.5f, hit.transform.position.y),
                    Mathf.Clamp(trans.position.z, float.MinValue, limitZPos));
                trans.position = clampedPos;
            }
            else
            {
                if (lastHitGround == null) return;
                Vector3 clampedPos = new Vector3(trans.position.x, 
                    Mathf.Clamp(trans.position.y, lastHitGround.position.y - 0.5f, lastHitGround.position.y),
                    Mathf.Clamp(trans.position.z, float.MinValue, limitZPos));
                trans.position = clampedPos;
            }
        }
        else
        {
            if (lastHitGround == null) return;
            Vector3 clampedPos = new Vector3(trans.position.x, 
                Mathf.Clamp(trans.position.y, lastHitGround.position.y - 0.5f, lastHitGround.position.y),
                Mathf.Clamp(trans.position.z, float.MinValue, limitZPos));
            trans.position = clampedPos;
        }
    }
}
