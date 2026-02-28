using UnityEngine;

public class KeyCard : MonoBehaviour
{
    public static bool hasKeyCard = false;
    public Animator doorAnimator;
    public AudioSource doorSound;
    public AudioClip pickup, unlock;

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 4f))
        {
            if (hit.collider.CompareTag("Key"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hasKeyCard = true;
                    Destroy(hit.collider.gameObject);
                    Debug.Log("🗝️ Keycard picked up!");
                    doorSound.PlayOneShot(pickup);
                }
            }
            else if (hit.collider.CompareTag("CheckKey") && hasKeyCard)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Open the door
                    doorAnimator.SetTrigger("OpenDoor");
                    Debug.Log("🚪 Door unlocked with keycard!");
                    doorSound.PlayOneShot(unlock);
                }
            }
        }
    }
}
