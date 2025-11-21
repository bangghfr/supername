using System.Collections;
using UnityEngine;

public class TunnelAnimation : MonoBehaviour
{
    public Sprite[] frames;
    public float frameTime = 0.12f;

    SpriteRenderer sr;
    int frame = 0;
    float timer = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > frameTime)
        {
            frame = (frame + 1) % frames.Length;
            sr.sprite = frames[frame];
            timer = 0;
        }
    }
}
