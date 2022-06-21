using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator anim;
    public Rigidbody rb;
    public Transform trans;
    public float speed;

    public DynamicJoystick joystick;

    private int animState;
    private string ANIM_ACTION = "Action";
    private const int IDLE_ANIM = 0;
    private const int RUN_ANIM = 1;

    void Update()
    {
        rb.velocity = new Vector3(joystick.Horizontal * speed, trans.position.y, joystick.Vertical * speed);

        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            SetAnim(RUN_ANIM);
            trans.rotation = Quaternion.LookRotation(rb.velocity);
        }
        else
        {
            SetAnim(IDLE_ANIM);
        }
    }

    private void SetAnim(int state)
    {
        if (animState != state)
        {
            animState = state;
            anim.SetInteger(ANIM_ACTION, animState);
        }
    }
}
