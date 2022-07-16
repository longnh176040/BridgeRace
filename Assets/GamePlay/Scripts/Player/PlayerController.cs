using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseActor
{
    public bool isDie;

    #region Singleton
    public static PlayerController ins;

    void Awake()
    {
        ins = this;
    }
    #endregion

    private void Start()
    {
        base.Start();
        id = 0;

        
    }

    void Update()
    {
        
    }
}
