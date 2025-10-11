using audio_subtitle_system;
using UnityEngine;

public class LookAtTrigger : MonoBehaviour
{
    [SerializeField] private float lookRange = 5f;
    [SerializeField] private string targetTag = "Interactable";

    private GameObject lastLookedObject;
    private DialogueTrigger dialogueTrigger;

    void Start()
    {
        dialogueTrigger = FindObjectOfType<DialogueTrigger>();
        if (dialogueTrigger == null)
        {
            Debug.LogError("DialogueTrigger not found in the scene.");
        }
    }

    void Update()
    {
        GameObject currentLook = null;
        
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, lookRange))
        {
            if (hit.collider.isTrigger && hit.collider.CompareTag(targetTag))
            {
                currentLook = hit.collider.gameObject;
            }
        }

        if (currentLook != lastLookedObject)
        {
            if (lastLookedObject != null)
                Debug.Log("Stopped looking at: " + lastLookedObject.name);

            if (currentLook != null)
            {
                Debug.Log("Looking at: " + currentLook.name);
                if(dialogueTrigger.isPlaying == false && dialogueTrigger.selectedJsonIndex == 1)
                {
                    dialogueTrigger.PlayAllDialoguesFromFile(2);
                }
            }
            
            lastLookedObject = currentLook;
        }
    }
}