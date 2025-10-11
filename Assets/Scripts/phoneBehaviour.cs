using UnityEngine;

public class phoneBehaviour : MonoBehaviour
{
    public AudioClip phoneSound;

    public void PlayPhoneSound()
    {
        if (phoneSound != null) AudioSource.PlayClipAtPoint(phoneSound, transform.position);
    }
    public void StopPhoneSound()
    {
        // Implement stop sound logic if needed
        GameObject.Destroy(this);
    }
}
