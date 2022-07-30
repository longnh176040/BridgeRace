using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public abstract class BaseActor : MonoBehaviour
{
    internal int id;
    internal bool isWin;

    public Transform brickContainer;
    public static UnityAction<int, int, int> CollectBrick;

    protected Stack<Brick> brickStack = new Stack<Brick>();
    [SerializeField]
    internal int currentStage;

    private float brickHeight = 0.35f;
    internal bool[] hasFinishedStage = new bool[3]; //3 tương ứng với số stage

    public Animator anim;
    protected int animState;

    protected void Start()
    {
        LevelManager.ins.SetupActorDictionary(this);
    }

    //public virtual void OnTriggerEnter(Collider other)
    //{ }

    public void SetAnim(int state)
    {
        if (animState != state)
        {
            animState = state;
            anim.SetInteger(Constant.ANIM_ACTION, animState);
        }
    }

    public int GetStackNum()
    {
        return brickStack.Count;
    }

    public void PushToStack(Brick brick)
    {
        brickStack.Push(brick);
        brick.trans.parent = brickContainer;
        brick.trans.localEulerAngles = Vector3.zero;
        brick.trans.DOLocalMove(new Vector3(0, (brickStack.Count - 1) * brickHeight, 0), 0.5f);
        brick.trans.DOLocalRotate(new Vector3(0, 360f, 360f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        brick.RemoveFromBrickList();
        CollectBrick(brick.id, brick.orderPos, currentStage);
    }

    public void PopFromStack()
    {
        if (brickStack.Count > 0)
        {
            Brick brick = brickStack.Pop();
            brick.trans.parent = null;
            brick.trans.localEulerAngles = Vector3.zero;
            brick.Reset();
            ObjectPooling.ins.EnQueueObj(Constant.BRICK_TAG, brick.gameObject);
        }
    }

    public void ClearStack()
    {
        foreach (Brick brick in brickStack)
        {
            brick.RemoveFromBrickList();
            brick.trans.parent = null;
            brick.Reset();
        }
        brickStack.Clear();
    }

    public void ClearWinStack()
    {
        foreach (Brick brick in brickStack)
        {
            brick.RemoveFromBrickList();
            brick.trans.parent = null;
            brick.LiableBrick();

            Vector3 forceDir = new Vector3(Random.Range(-8f, 8f), Random.Range(5f, 10f), Random.Range(-8f, 8f));
            brick.rb.AddForce(forceDir.normalized * Random.Range(500f, 1000f), ForceMode.Force);
            brick.rb.AddTorque(transform.up * 100f * Random.Range(-1, 1));
            Destroy(brick.gameObject, 5f);
        }
        brickStack.Clear();
    }
}
