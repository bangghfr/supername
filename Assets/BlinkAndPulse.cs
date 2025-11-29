using UnityEngine;

public class BlinkAndPulse : MonoBehaviour
{
    [Header("Blink Settings")]
    public float minAlpha = 0.3f;
    public float maxAlpha = 1f;
    public float blinkSpeed = 2f;

    [Header("Pulse Settings")]
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public float pulseSpeed = 2f;

    private SpriteRenderer sr;
    private float alphaT = 0f;
    private float scaleT = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // ------- МИГАНИЕ -------
        alphaT += Time.deltaTime * blinkSpeed;
        float a = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(alphaT) + 1) / 2f);

        if (sr != null)
        {
            Color c = sr.color;
            c.a = a;
            sr.color = c;
        }

        // ------- ПУЛЬСАЦИЯ (изменение размера) -------
        scaleT += Time.deltaTime * pulseSpeed;
        float s = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(scaleT) + 1) / 2f);

        transform.localScale = new Vector3(s, s, 1);
    }
}
