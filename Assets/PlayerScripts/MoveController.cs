using UnityEngine;

[RequireComponent(typeof(InputReceiver))]
[RequireComponent(typeof(Rigidbody2D))]
public class MoveController : MonoBehaviour
{
    // ================== REFERENCES ==================
    private InputReceiver input;
    private Rigidbody2D rb;

    // ================== PLAYER STATE ==================
    public int _playerHp = 100;

    private bool isGrounded;
    private bool isHooked;
    private bool isTouchingWall;
    private int wallDir;

    // ================== MOVEMENT ==================
    [Header("Ground Movement")]
    public float moveSpeed = 6f;
    public float groundFriction = 20f;

    [Header("Air Control (DDNet)")]
    public float airAcceleration = 35f;
    public float airMaxSpeed = 6f;
    public float airDrag = 8f;
    public float hookParallelControl = 1f;
    public float hookPerpendicularControl = 0.35f;

    // ================== JUMP ==================
    [Header("Jump")]
    public float jumpForce = 8f;
    public int maxJumps = 2;
    public float firstJumpMultiplier = 1.25f;
    private int jumpsLeft;

    // ================== WALL ==================
    [Header("Wall")]
    public float wallSlideSpeed = 2f;
    public float wallJumpForce = 10f;

    // ================== HOOK ==================
    [Header("Hook")]
    public float hookMaxDistance = 14f;
    public float hookMinDistance = 2f;
    public float hookPullForce = 22f;
    public float hookReleaseBoost = 8f;
    public float hookSideForce = 18f;

    private Vector2 hookPoint;
    private float ropeLength;

    // ================== DASH ==================
    [Header("Dash")]
    public float dashForce = 14f;
    public float superDashForce = 25f;
    public float superDashCooldown = 15f;
    private float lastSuperDashTime;

    // ================== UNITY ==================
    void Awake()
    {
        input = GetComponent<InputReceiver>();
        rb = GetComponent<Rigidbody2D>();
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        HandleGroundMovement();
        HandleJump();
        HandleHookInput();
        HandleDash();
    }

    void FixedUpdate()
    {
        ApplyAirControl();
        ApplyGroundFriction();
        ApplyHookPhysics();
        HandleWallSlide();
    }

    // ================== GROUND ==================
    void HandleGroundMovement()
    {
        if (!isGrounded) return;
        rb.velocity = new Vector2(input.Move.x * moveSpeed, rb.velocity.y);
    }

    void ApplyGroundFriction()
    {
        if (isGrounded && Mathf.Abs(input.Move.x) < 0.01f)
        {
            rb.velocity = new Vector2(
                Mathf.Lerp(rb.velocity.x, 0f, groundFriction * Time.fixedDeltaTime),
                rb.velocity.y
            );
        }
    }

    // ================== AIR CONTROL ==================
    void ApplyAirControl()
    {
        if (isGrounded) return;

        float inputX = input.Move.x;
        if (Mathf.Abs(inputX) < 0.01f) return;

        float controlMultiplier = 1f;

        if (isHooked)
        {
            Vector2 ropeDir = (hookPoint - (Vector2)transform.position).normalized;
            Vector2 moveDir = Vector2.right * Mathf.Sign(inputX);
            float dot = Mathf.Abs(Vector2.Dot(ropeDir, moveDir));
            controlMultiplier = Mathf.Lerp(hookPerpendicularControl, hookParallelControl, dot);
        }

        rb.AddForce(Vector2.right * inputX * airAcceleration * controlMultiplier, ForceMode2D.Force);

        float clampedX = Mathf.Clamp(rb.velocity.x, -airMaxSpeed, airMaxSpeed);
        rb.velocity = new Vector2(clampedX, rb.velocity.y);
    }

    // ================== JUMP ==================
    void HandleJump()
    {
        if (!input.JumpPressed) return;

        if (isTouchingWall && !isGrounded)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(-wallDir, 1f) * wallJumpForce, ForceMode2D.Impulse);
            return;
        }

        if (jumpsLeft > 0)
        {
            float mult = jumpsLeft == maxJumps ? firstJumpMultiplier : 1f;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce * mult, ForceMode2D.Impulse);
            jumpsLeft--;
        }
    }

    // ================== WALL ==================
    void HandleWallSlide()
    {
        if (!isGrounded && isTouchingWall && rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
    }

    // ================== HOOK ==================
    void HandleHookInput()
    {
        if (input.AttackPressed && !isHooked)
            TryHook();

        if (!input.AttackHeld && isHooked)
        {
            Vector2 boostDir = (hookPoint - (Vector2)transform.position).normalized;
            rb.AddForce(boostDir * hookReleaseBoost, ForceMode2D.Impulse);
            isHooked = false;
        }
    }

    void TryHook()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector2 world = Camera.main.ScreenToWorldPoint(mouse);

        Vector2 dir = world - (Vector2)transform.position;
        if (dir.magnitude > hookMaxDistance)
            dir = dir.normalized * hookMaxDistance;

        int mask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude, mask);

        if (hit.collider == null) return;

        hookPoint = hit.point;
        ropeLength = Mathf.Clamp(Vector2.Distance(transform.position, hookPoint), hookMinDistance, hookMaxDistance);
        isHooked = true;
    }

    void ApplyHookPhysics()
    {
        if (!isHooked) return;

        Vector2 pos = transform.position;
        float dist = Vector2.Distance(pos, hookPoint);

        if (dist < ropeLength)
            ropeLength = Mathf.Clamp(dist, hookMinDistance, hookMaxDistance);

        if (dist > ropeLength)
        {
            Vector2 dir = (hookPoint - pos).normalized;
            float pull = hookPullForce + (dist - ropeLength) * 6f;
            rb.AddForce(dir * pull, ForceMode2D.Force);
        }

        Vector2 mouseDir = ((Vector2)Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)))
            - pos).normalized;

        rb.AddForce(mouseDir * hookSideForce, ForceMode2D.Force);
    }

    // ================== DASH ==================
    void HandleDash()
    {
        if (!Input.GetKeyDown(KeyCode.LeftShift)) return;

        int dir = (int)Mathf.Sign(transform.localScale.x);

        if (Time.time >= lastSuperDashTime + superDashCooldown)
        {
            rb.AddForce(Vector2.right * dir * superDashForce, ForceMode2D.Impulse);
            lastSuperDashTime = Time.time;
        }
        else
        {
            rb.AddForce(Vector2.right * dir * dashForce, ForceMode2D.Impulse);
        }
    }

    // ================== COLLISIONS ==================
    void OnCollisionEnter2D(Collision2D c)
    {
        foreach (var contact in c.contacts)
        {
            if (contact.normal.y > 0.4f)
            {
                isGrounded = true;
                jumpsLeft = maxJumps;
            }

            if (Mathf.Abs(contact.normal.x) > 0.9f)
            {
                isTouchingWall = true;
                wallDir = (int)Mathf.Sign(contact.normal.x);
            }
        }
    }

    void OnCollisionExit2D(Collision2D c)
    {
        isGrounded = false;
        isTouchingWall = false;
    }

    // ================== HELPERS ==================
    public bool IsHooked() => isHooked;
    public Vector2 GetHookPoint() => hookPoint;

    // ================== COOLDOWNS FOR UI ==================
    public float GetDashCooldown01()
    {
        return Mathf.Clamp01(1f); // обычный деш без кулдауна
    }

    public float GetSuperDashCooldown01()
    {
        return Mathf.Clamp01((Time.time - lastSuperDashTime) / superDashCooldown);
    }

}
