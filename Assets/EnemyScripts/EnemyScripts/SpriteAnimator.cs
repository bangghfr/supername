using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public float frameRate = 0.1f;

    private Sprite[] frames;
    private int frameIndex;
    private float timer;
    private bool playing;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Play(Sprite[] newFrames, float newFrameRate, bool loop = true)
    {
        if (frames == newFrames) return;

        frames = newFrames;
        frameRate = newFrameRate;
        frameIndex = 0;
        timer = 0f;
        playing = true;

        spriteRenderer.sprite = frames[0];
        this.loop = loop;
    }

    private bool loop;

    private void Update()
    {
        if (!playing || frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0f;
            frameIndex++;

            if (frameIndex >= frames.Length)
            {
                if (loop)
                    frameIndex = 0;
                else
                {
                    frameIndex = frames.Length - 1;
                    playing = false;
                }
            }

            spriteRenderer.sprite = frames[frameIndex];
        }
    }

    public bool IsPlaying()
    {
        return playing;
    }
}
