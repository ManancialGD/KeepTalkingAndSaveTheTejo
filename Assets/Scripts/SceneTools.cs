using UnityEngine;

public class SceneTools : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        // Load the new scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
}