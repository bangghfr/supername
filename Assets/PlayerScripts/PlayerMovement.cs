using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private InputReceiver inputReceiver;
    private MoveController controller;

    [Header("Rope Visual")]
    public Sprite ropeSpriteAsset;
    private Transform ropeSprite;

    void Start()
    {
        inputReceiver = GetComponent<InputReceiver>();
        controller = GetComponent<MoveController>();

        if (ropeSpriteAsset != null)
        {
            GameObject rope = new GameObject("RopeSprite");
            SpriteRenderer sr = rope.AddComponent<SpriteRenderer>();
            sr.sprite = ropeSpriteAsset;
            sr.sortingOrder = 50;
            ropeSprite = rope.transform;
            ropeSprite.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        HandleFlip();
        DrawRope();
    }

    private void HandleFlip()
    {
        float x = inputReceiver.Move.x;

        if (x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void DrawRope()
    {
        if (ropeSprite == null) return;

        if (!controller.IsHooked())
        {
            ropeSprite.gameObject.SetActive(false);
            return;
        }

        ropeSprite.gameObject.SetActive(true);

        Vector2 start = transform.position;
        Vector2 end = controller.GetHookPoint();

        ropeSprite.position = (start + end) / 2f;

        Vector2 diff = end - start;
        ropeSprite.localScale = new Vector3(diff.magnitude, 0.07f, 1f);

        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        ropeSprite.rotation = Quaternion.Euler(0, 0, angle);
    }
}
