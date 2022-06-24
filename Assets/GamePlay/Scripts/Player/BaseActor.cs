﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class BaseActor : MonoBehaviour
{
    internal int id;

    public Transform brickContainer;

    protected Stack<Brick> brickStack = new Stack<Brick>();

    private float brickHeight = 0.35f;

    protected void Start()
    {
        LevelManager.ins.SetupActorDictionary(this);
    }

    public void PushToStack(Brick brick)
    {
        brickStack.Push(brick);
        brick.trans.parent = brickContainer;
        brick.trans.localEulerAngles = Vector3.zero;
        brick.trans.DOLocalMove(new Vector3(0, (brickStack.Count - 1) * brickHeight, 0), 0.5f);
        brick.trans.DOLocalRotate(new Vector3(0, 360f, 360f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
    }

    public void PopFromStack()
    {
        Brick brick = brickStack.Pop();
        brick.trans.parent = null;
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