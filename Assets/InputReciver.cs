using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{
   
    public Vector2 Move { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool AtackPressed { get; private set; }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Move = new Vector2(moveX, moveY);

       
        JumpPressed = Input.GetButtonDown("Jump");
        AtackPressed = Input.GetMouseButtonDown(0);
    }
}
//bool  { get; }

