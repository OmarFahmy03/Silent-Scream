using UnityEngine;
using UnityEngine.Playables;

public class EndGame : MonoBehaviour
{
    public PlayableDirector endGameCutscene;
    public AudioSource Ambience;

    void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2f))
        {
            if (hit.collider.CompareTag("EndDoor"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    EndGameCutscene();
                    Ambience.Stop();
                }
            }
        }
    }

    public void EndGameCutscene()
    {
        endGameCutscene.Play();
        Invoke(nameof(QuitGame), 15f); // Adjust delay as needed
    }
    void QuitGame()
    {
        Debug.Log("Game Over. Thanks for playing!");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
