using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
        private InputReceiver inputReceiver;

        void Start()
        {
            inputReceiver = GetComponent<InputReceiver>();
        }

        void Update()
        {
            float moveX = inputReceiver.Move.x;

            if (moveX > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveX < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            // Остальная логика движения ...
        }
}
