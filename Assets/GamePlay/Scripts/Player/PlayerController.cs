using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseActor
{
    public PlayerMovement playerMovement;

    #region Singleton
    public static PlayerController ins;

    void Awake()
    {
        ins = this;
    }
    #endregion

    private void Start()
    {
        id = 0;
        currentStage = 1;
        base.Start();
    }

    public void SetWin(int rank)
    {
        playerMovement.canMove = false;
        playerMovement.rb.isKinematic = true;
        ClearWinStack();

        Transform winTrans = MapManager.ins.winPos[rank];
        Vector3 winPos = new Vector3(winTrans.position.x, playerMovement.trans.position.y, winTrans.position.z);
        playerMovement.trans.position = winPos;
        playerMovement.transRotate.localEulerAngles = Vector3.up * 180f;

        SetAnim(Constant.WIN_ANIM);
    }
}
