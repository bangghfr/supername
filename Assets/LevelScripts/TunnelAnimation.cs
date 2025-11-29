using System.Collections;
using UnityEngine;
public class TunnelAnimation : MonoBehaviour
{
    public Sprite[] frames;
    public float frameTime = 0.12f;

    private SpriteRenderer sr;
    private int frame = 0;
    private float timer = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) Debug.LogWarning("TunnelAnimation: no SpriteRenderer attached.");
        if (frames == null || frames.Length == 0) Debug.LogWarning("TunnelAnimation: no frames assigned.");
    }

    void Update()
    {
        if (frames == null || frames.Length == 0 || sr == null) return;

        timer += Time.deltaTime;
        if (timer >= frameTime)
        {
            frame = (frame + 1) % frames.Length;
            sr.sprite = frames[frame];
            timer = 0f;
        }
    }
}
