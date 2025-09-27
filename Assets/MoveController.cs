using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

[RequireComponent(typeof(InputReceiver))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private InputReceiver inputReceiver;
    private Rigidbody2D rb;
    private bool isGrounded = true; // Для простоты — считать, что персонаж стоит на земле

    void Awake()
    {
        inputReceiver = GetComponent<InputReceiver>();
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D компонент отсутствует на объекте!");
        }
    }

    void Update()
    {
        MovePlayer();
        HandleJump();
        HandleAttack();
    }

    private void MovePlayer()
    {
        Vector2 movement = inputReceiver.Move * moveSpeed;
        // Перемещение по горизонтали (x)
        rb.velocity = new Vector2(movement.x, rb.velocity.y);
    }

    private void HandleJump()
    {
        if (inputReceiver.JumpPressed && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void HandleAttack()
    {
        if (inputReceiver.AtackPressed)
        {
            Debug.Log("Атака игрока!");
            // Добавьте сюда логику атаки персонажа

        }
    }

    // Простейший метод для определения касания земли (упрощённо)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}
