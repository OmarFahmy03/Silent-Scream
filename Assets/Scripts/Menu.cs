using UnityEngine;

public class Menu : MonoBehaviour
{
    public CanvasGroup CG;
    public void Quit()
    {
        Application.Quit();
    }
    public void StartGame()
    {
        StartCoroutine(FadeAndLoadScene());
    }
    private System.Collections.IEnumerator FadeAndLoadScene()
    {
        float duration = 1.5f; // Duration of the fade
        float elapsed = 0f;

        while (elapsed < duration)
        {
            CG.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        CG.alpha = 1f; // Ensure it's fully opaque at the end
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
}
