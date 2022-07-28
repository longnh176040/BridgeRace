using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 offset;

    public Transform mainCamera;
    public PlayerMovement player;

    private void LateUpdate()
    {
        if (!PlayerController.ins.isWin)
        {
            mainCamera.position = player.trans.position - offset;
            mainCamera.LookAt(player.trans.position + Vector3.up);
        }
    }
}
