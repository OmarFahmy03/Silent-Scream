using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ChromaticAberrationController : MonoBehaviour
{
    public PostProcessVolume volume;
    private ChromaticAberration chromaticAberration;
    public GameObject MonsterPrefab; // Assign in inspector
    private Transform PlayerTransform;
    public AudioClip scareSound; // Assign in inspector

    void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Car").transform;
        if (volume != null)
        {
            volume.profile.TryGetSettings(out chromaticAberration);
            Debug.Log("Chromatic Aberration effect found and ready to use.");
        }
    }

    public IEnumerator SetChromaticAberration()
    {
        if (chromaticAberration != null)
        {
            float duration = 1.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(0f, 1f, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            AudioSource.PlayClipAtPoint(scareSound, PlayerTransform.position);
            GameObject monster = Instantiate(MonsterPrefab, PlayerTransform.position + new Vector3(0, 0, 20), Quaternion.Euler(0, 180, 0));
            chromaticAberration.intensity.value = 1f;
        }
        yield return new WaitForSeconds(1f);
        if (chromaticAberration != null)
        {
            float duration = 1.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(1f, .5f, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            chromaticAberration.intensity.value = 0f;
        }
        yield return new WaitForSeconds(1f);
        if (chromaticAberration != null)
        {
            float duration = 1.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(.5f, 1f, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            chromaticAberration.intensity.value = 1f;
        }
        yield return new WaitForSeconds(1f);
        if (chromaticAberration != null)
        {
            float duration = 1.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(1f, 0f, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            chromaticAberration.intensity.value = 0f;
        }
    }
}
