using UnityEngine;

[RequireComponent(typeof(Light))]
public class SimpleFlicker : MonoBehaviour
{
    [Header("Light")]
    public Light targetLight;
    public float baseIntensity = 1f;

    [Header("Flicker Settings")]
    public float minIntensity = 0f;
    public float maxIntensity = 1f;
    public float changeSpeed = 0.05f; // how fast intensity changes (seconds)
    public float flickerChance = 0.25f; // chance each tick to change intensity

    void Reset()
    {
        targetLight = GetComponent<Light>();
    }

    void Start()
    {
        if (targetLight == null) targetLight = GetComponent<Light>();
        baseIntensity = targetLight.intensity;
        minIntensity = Mathf.Clamp(minIntensity, 0f, baseIntensity);
        maxIntensity = Mathf.Max(maxIntensity, minIntensity);
        StartCoroutine(FlickerRoutine());
    }

    System.Collections.IEnumerator FlickerRoutine()
    {
        while (true)
        {
            if (Random.value < flickerChance)
            {
                float newIntensity = Random.Range(minIntensity, maxIntensity);
                targetLight.intensity = newIntensity;
            }
            else
            {
                // sometimes return to base
                targetLight.intensity = Mathf.Lerp(targetLight.intensity, baseIntensity, 0.2f);
            }

            yield return new WaitForSeconds(changeSpeed);
        }
    }
}
