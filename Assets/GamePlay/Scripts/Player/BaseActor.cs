using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public abstract class BaseActor : MonoBehaviour
{
    internal int id;

    public Transform brickContainer;
    public static UnityAction<int, int> CollectBrick;

    protected Stack<Brick> brickStack = new Stack<Brick>();

    private float brickHeight = 0.35f;

    protected void Start()
    {
        LevelManager.ins.SetupActorDictionary(this);
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

        CollectBrick(brick.id, brick.orderPos);
    }

    public void PopFromStack()
    {
        Brick brick = brickStack.Pop();
        brick.trans.parent = null;
        ObjectPooling.ins.EnQueueObj(Constant.BRICK_TAG, brick.gameObject);
    }

    public void ClearStack()
    {
        foreach (Brick brick in brickStack)
        {
            brick.trans.parent = null;
            brick.Reset();
        }
        brickStack.Clear();
    }
}
