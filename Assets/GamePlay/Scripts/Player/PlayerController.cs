using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isDie;
    #region Singleton
    public static PlayerController ins;

    void Awake()
    {
        ins = this;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }
}
