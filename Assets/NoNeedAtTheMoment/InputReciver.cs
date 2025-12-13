using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool AttackHeld { get; private set; }
    public bool AttackReleased { get; private set; }

    void Update()
    {
        // Движение
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Move = new Vector2(moveX, moveY);

        // Прыжок
        JumpPressed = Input.GetButtonDown("Jump");

        // Атака / крюк
        AttackPressed = Input.GetMouseButtonDown(0);
        AttackHeld = Input.GetMouseButton(0);
        AttackReleased = Input.GetMouseButtonUp(0);
    }
}
