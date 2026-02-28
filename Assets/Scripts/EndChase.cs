using UnityEngine;

public class EndChase : MonoBehaviour
{
    public AudioSource ChaseEndSound;
    public AudioSource newAmbience;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🏁 Player reached the safe zone! Chase ended.");
            // Implement additional logic for ending the chase, e.g., stop monster, play sound, etc.
            GameObject monster = GameObject.FindWithTag("Monster");
            if (monster != null)
            {
                Destroy(monster);
            }
            StartCoroutine(ChaseEnd());
        }
    }

    private System.Collections.IEnumerator ChaseEnd()
    {
        if (ChaseEndSound != null)
        {
            // Play the sound
            ChaseEndSound.Play();
            float fadeDuration = 2f;
            float startVolume = ChaseEndSound.volume;
            float elapsedTime = 0f;
            
            // Fade out the audio over fadeDuration
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                ChaseEndSound.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
                yield return null;
            }
            
            // Ensure volume is set to 0 at the end
            ChaseEndSound.volume = 0f;
            ChaseEndSound.Stop();
            
            // Reset volume for next use
            ChaseEndSound.volume = startVolume;
        }

        // Additional logic after sound ends, if needed
        Debug.Log("Chase end sequence complete!");
        if (newAmbience != null)
        {
            newAmbience.Play();
        }
    }
}