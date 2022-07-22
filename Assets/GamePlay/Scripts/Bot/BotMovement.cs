using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotMovement : MonoBehaviour
{
    public BaseActor actor;
    public Transform trans;
    public Transform transRotate;
    public NavMeshAgent agent;

    public Bot_State botState;

    [SerializeField]
    internal Transform curTarget;
    private float closeThreshold = 1f;

    [SerializeField]
    internal int pickedBridgeId = -1;
    public Transform stepCheck;
    public float stepOffset = 2f;
    private Stage currentStage;
    [SerializeField]
    private Transform currentBridgeTarget;
    //private Transform lastHitGround;
    private float limitMinZPos = float.MinValue;
    private float limitMaxZPos = float.MaxValue;

    internal bool isOnBridge = false;

    void Start()
    {
        StartCoroutine(IE_ChangeOnBridgeState());
    }

    void Update()
    {
        if (botState == Bot_State.Collect)
        {
            MoveToTarget();
        }
        else if (botState == Bot_State.OnBridge)
        {
            OnBridge();
        }
        FaceTarget();
    }

    #region Collect
    public void FindRandomBrick()
    {
        int brickCount = BrickSpawner.ins.brickLists[actor.id].Count;
        if (brickCount > 0)
        {
            float minDist = float.MaxValue;
            int tmp = 0;
            float dist;

            for (int i = 0; i < brickCount; i++)
            {
                dist = (BrickSpawner.ins.brickLists[actor.id][i].trans.position - trans.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    tmp = i;
                }
            }
            curTarget = BrickSpawner.ins.brickLists[actor.id][tmp].trans;
            actor.SetAnim(Constant.RUN_ANIM);
        }
        else 
        {
            curTarget = null;
            actor.SetAnim(Constant.IDLE_ANIM);
        }
    }

    void MoveToTarget()
    {
        isOnBridge = false;

        if (curTarget != null)
        {
            if ((curTarget.position - trans.position).sqrMagnitude < closeThreshold)
            {
                //Debug.Log("789");
                FindRandomBrick();
            }
            else
            {
                //Debug.Log("000");
                agent.SetDestination(curTarget.position);
            }
        }
        else
        {
            FindRandomBrick();
        }
    }

    void FaceTarget()
    {
        var turnTowardNavSteeringTarget = agent.steeringTarget;

        Vector3 direction = (turnTowardNavSteeringTarget - trans.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transRotate.rotation = Quaternion.Slerp(transRotate.rotation, lookRotation, Time.deltaTime * 5);
    }
    #endregion

    #region OnBridge
    void OnBridge()
    {
        //if (pickedBridgeId == -1)
        //{
        //    ChooseBridge();
        //}
        //else
        //{
            CheckSpawnBridge();
            MoveOnBridge();
            ClimbBridge();
        //}
    }

    void ChooseBridge()
    {
        Debug.Log("Get new bridge");
        currentStage = MapManager.stageDict[actor.currentStage];

        float rand;
        for (int i = 0; i < currentStage.isGatesChosen.Length; i++)
        {
            if (currentStage.isGatesChosen[i])
            {
                rand = Random.Range(0, 1f);
                if (rand > 0.5f)
                {
                    pickedBridgeId = i;
                }
            }
            else
            {
                if (pickedBridgeId == -1) pickedBridgeId = i;

                rand = Random.Range(0, 1f);
                if (rand > 0.5f)
                {
                    pickedBridgeId = i;
                }
            }
        }
        
    }

    void MoveOnBridge()
    {
        if (currentBridgeTarget != null)
        {
            float dist = (trans.position - currentBridgeTarget.position).sqrMagnitude;

            if (dist < closeThreshold)
            {
                currentBridgeTarget = currentStage.endGates[pickedBridgeId];
                agent.SetDestination(currentBridgeTarget.position);
            }
        }
    }

    private void CheckSpawnBridge()
    {
        RaycastHit hit;
        if (Physics.Raycast(stepCheck.position, Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag(Constant.BRIDGE_TAG))
            {
                Debug.Log("hit bridge");

                isOnBridge = true;
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
                        pickedBridgeId = -1;

                        float targetGroundSize = b.targetGround.GetComponent<Collider>().bounds.size.z / 2;
                        limitMaxZPos = float.MaxValue;
                        limitMinZPos = b.targetGround.position.z - targetGroundSize + 0.25f;

                        b.isBuilt = true;
                        BrickSpawner.ins.DequeueUnuseBrick(actor.id, actor.currentStage);
                        actor.currentStage++;
                        BrickSpawner.ins.InitMapWithId(actor.id, actor.currentStage);
                        ChangeState(Bot_State.Collect);
                        StartCoroutine(IE_ChangeOnBridgeState());
                    }
                }
                else if (actor.GetStackNum() == 0) {
                    if (botState == Bot_State.OnBridge)
                    {
                        ChangeState(Bot_State.Collect);
                        StartCoroutine(IE_ChangeOnBridgeState());
                    }
                }
            }
            else
            {
                Debug.Log("hit " + hit.transform.name);
            }
        }
    }

    private void ClimbBridge()
    {
        RaycastHit hit;
        if (Physics.Raycast(stepCheck.position, trans.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag(Constant.BRIDGE_BRICK_TAG))
            {
                //lastHitGround = hit.transform;
                trans.position += new Vector3(0f, stepOffset * Time.fixedDeltaTime, 0f);

                if (actor.GetStackNum() == 0)
                {
                    limitMaxZPos = hit.transform.position.z + hit.collider.bounds.size.z / 2;
                    Vector3 clampedPos = new Vector3(trans.position.x,
                        Mathf.Clamp(trans.position.y, hit.transform.position.y + hit.collider.bounds.size.y/2, hit.transform.position.y),
                        Mathf.Clamp(trans.position.z, limitMinZPos, limitMaxZPos));
                    trans.position = clampedPos;

                    //Vector3 height = new Vector3(0, hit.transform.position.y + hit.collider.bounds.size.y / 2, 0);
                    //transRotate.position = height;
                    Debug.Log("1");
                }
            }
            //else if (hit.transform.CompareTag(Constant.GROUND_TAG))
            //{
            //    trans.position = Vector3.zero;
            //}
        }
    }

    //void OnDrawGizmos()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(stepCheck.position, trans.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
    //    {
    //        Gizmos.color = Color.red;
    //        Vector3 direction = trans.TransformDirection(Vector3.down) * Mathf.Infinity;
    //        Gizmos.DrawRay(stepCheck.position, direction);
    //    }
    //}
    #endregion

    IEnumerator IE_ChangeOnBridgeState()
    {
        yield return Yielders.Get(Random.Range(5f, 10f));

        if (actor.GetStackNum() != 0)
        {
            ChangeState(Bot_State.OnBridge);

            if (pickedBridgeId == -1)
            {
                ChooseBridge();
            }

            currentBridgeTarget = currentStage.bridge[pickedBridgeId];
            agent.SetDestination(currentBridgeTarget.position);
            Debug.Log("111111111");
        }
        else
        {
            StartCoroutine(IE_ChangeOnBridgeState());
        }
    }

    IEnumerator IE_ChangeState(Bot_State state)
    {
        yield return Yielders.Get(Random.Range(5f, 10f));
        ChangeState(state);
    }

    void ChangeState(Bot_State state)
    {
        if (botState != state)
        {
            botState = state;
        }
    }
}

public enum Bot_State
{
    Collect, 
    OnBridge
}
