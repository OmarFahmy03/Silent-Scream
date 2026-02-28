using UnityEngine;
using UnityEngine.UI;

public class NoteSystem : MonoBehaviour
{
    public GameObject NoteItself; // the note on the wall
    public GameObject Note_GameObject; // The Whole Note image
    public Text NoteText; // the text to change here
    public AudioSource Source; // to play audios
    public AudioClip PaperSound;
    public AudioClip SH2Soundeffect;
    public FirstPersonController Fpscontroller;
    public Text InteractionText;

    private bool isReading = false;
    public CanvasGroup DeathCanvasGroup; // Assign in inspector

    void Update()
    {
        Ray ray1 = new Ray(transform.position, transform.forward);
        RaycastHit hit1;

        // When player is not reading
        if (!isReading)
        {
            if (Physics.Raycast(ray1, out hit1, 1.5f))
            {
                if (hit1.collider.CompareTag("Paper"))
                {
                    InteractionText.text = "Press E to Read the Note";

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        OpenNote();
                    }
                }
            }
            else
            {
                InteractionText.text = "";
            }
        }
        // When player is reading
        else
        {
            InteractionText.text = "Press E to Close the Note";

            if (Input.GetKeyDown(KeyCode.E))
            {
                CloseNote();
            }
        }
    }

    void OpenNote()
    {
        isReading = true;
        Note_GameObject.SetActive(true);
        NoteText.text = 
        @"On my <b>right</b>, is where it all started.
In the back of the hallway, I heard screaming — it was on the same line as my room.
On the opposite side of the corridor, near my <b>escape route</b>, stands the <color=red>next target</color>. His room faced that of the previous patient.
Look <b>behind you</b> — you’ll find the two rooms you need. The first is near the <color=red>monster</color>.
I keep hearing his <b>groan</b>... he’s coming for <color=red>YOU</color>.
It all ends with <b><color=red>YOU</color></b>, and where you wake up.";


        Source.PlayOneShot(PaperSound);
        Fpscontroller.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseNote()
    {
        isReading = false;
        Note_GameObject.SetActive(false);
        NoteText.text = "";

        //Source.PlayOneShot(SH2Soundeffect);
        Fpscontroller.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InteractionText.text = "";
    }

    public void Death()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            //anim.SetTrigger("Death");
            DeathCanvasGroup.alpha = 1f; // Show the death canvas
        }
        Invoke("ResetScene", 3f); // wait for 3 seconds before reloading
    }
    void ResetScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
