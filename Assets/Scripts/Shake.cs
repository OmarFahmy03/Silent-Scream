using UnityEngine;

public class AlwaysCameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeIntensity = 0.1f;
    public float shakeFrequency = 20f;

    private Vector3 initialPosition;
    private float shakeTimer = 0f;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        shakeTimer += Time.deltaTime * shakeFrequency;

        float offsetX = Mathf.PerlinNoise(shakeTimer, 0f) * 2f - 1f;
        float offsetY = Mathf.PerlinNoise(0f, shakeTimer) * 2f - 1f;

        Vector3 shakeOffset = new Vector3(offsetX, offsetY, 0f) * shakeIntensity;
        transform.position = initialPosition + shakeOffset;
    }
}
