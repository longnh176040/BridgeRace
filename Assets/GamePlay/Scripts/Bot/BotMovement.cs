using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class BotMovement : MonoBehaviour
{
    public BaseActor actor;
    public Transform trans;
    public Transform transRotate;
    public NavMeshAgent agent;

    public Bot_State botState;
    public LayerMask bridgeLayer;

    internal Transform curTarget;
    private float closeThreshold = 1f;

    internal int pickedBridgeId = -1;
    public Transform stepCheck;
    public float stepOffset = 2f;
    private Stage currentStage;
    private float onBridgeY = 0.86f;

    private Transform currentBridgeTarget;
    private Bridge curBridge;

    internal bool isOnBridge = false;
    private bool hasNormalizedY = false;

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

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(collision.gameObject.name);
    //}

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
        if (hasNormalizedY)
        {
            OnGroundCheck();
        }

        if (curTarget != null)
        {
            if ((curTarget.position - trans.position).sqrMagnitude < closeThreshold)
            {
                FindRandomBrick();
            }
            else
            {
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

    private void OnGroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(stepCheck.position, trans.TransformDirection(Vector3.down), out hit, Mathf.Infinity, bridgeLayer))
        {
            if (hit.transform.CompareTag(Constant.GROUND_TAG))
            {
                hasNormalizedY = false;
                transRotate.DOLocalMove(Vector3.zero, 0.5f);
            }
        }
    }
    #endregion

    #region OnBridge
    void OnBridge()
    {
        CheckSpawnBridge();
        MoveOnBridge();
        if (!hasNormalizedY)
        {
            ClimbBridge();
        }
    }

    void ChooseBridge()
    {
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
        if (Physics.Raycast(stepCheck.position, Vector3.down, out hit, Mathf.Infinity, bridgeLayer))
        {
            if (hit.transform.CompareTag(Constant.BRIDGE_TAG))
            {
                isOnBridge = true;
                curBridge = Cache.GetBridge(hit.collider);

                if (actor.GetStackNum() > 0 && !curBridge.CheckBuildDone())
                {
                    curBridge.SpawnBridgeBrick(actor.id);
                    actor.PopFromStack();
                }
                else if (curBridge.CheckBuildDone())
                {
                    if (!actor.hasFinishedStage[actor.currentStage])
                    {
                        actor.hasFinishedStage[actor.currentStage] = true;

                        pickedBridgeId = -1;

                        BrickSpawner.ins.DequeueUnuseBrick(actor.id, actor.currentStage);

                        actor.currentStage++;
                        ChooseBridge();
                        BrickSpawner.ins.InitMapWithId(actor.id, actor.currentStage);
                        ChangeState(Bot_State.Collect);
                        FindRandomBrick();
                        StartCoroutine(IE_ChangeOnBridgeState());
                        //Debug.Log("===============Build done===============");
                    }
                }
                else if (actor.GetStackNum() == 0) {
                    if (botState == Bot_State.OnBridge)
                    {
                        ChangeState(Bot_State.Collect);
                        FindRandomBrick();
                        StartCoroutine(IE_ChangeOnBridgeState());
                    }
                }
            }
            else if (hit.transform.CompareTag(Constant.BRIDGE_BRICK_TAG))
            {
                //CheckOwner
                curBridge = hit.transform.GetComponent<BridgeBrick>().onBridge;
                BridgeBrick bridgeBrick = curBridge.bridgeBrickDict[hit.collider];

                if (actor.GetStackNum() > 0 && bridgeBrick.id != actor.id)
                {
                    bridgeBrick.ChangeOwner(actor.id);
                    actor.PopFromStack();
                }
                else if (actor.GetStackNum() <= 0)
                {
                    if (botState == Bot_State.OnBridge)
                    {
                        ChangeState(Bot_State.Collect);
                        FindRandomBrick();
                        StartCoroutine(IE_ChangeOnBridgeState());
                    }
                }
            }
        }
    }

    private void ClimbBridge()
    {
        RaycastHit hit;
        if (Physics.Raycast(stepCheck.position, trans.TransformDirection(Vector3.down), out hit, Mathf.Infinity, bridgeLayer))
        {
            if (hit.transform.CompareTag(Constant.BRIDGE_BRICK_TAG))
            {
                hasNormalizedY = true;
                transRotate.DOLocalMove(new Vector3(0, onBridgeY, 0), 0.5f);
            }
        }
    }
    #endregion

    IEnumerator IE_ChangeOnBridgeState()
    {
        yield return Yielders.Get(Random.Range(8f, 20f));

        if (actor.GetStackNum() != 0)
        {
            ChangeState(Bot_State.OnBridge);

            if (pickedBridgeId == -1)
            {
                ChooseBridge();
            }

            currentBridgeTarget = currentStage.bridge[pickedBridgeId];
            agent.SetDestination(currentBridgeTarget.position);
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
