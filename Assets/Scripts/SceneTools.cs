using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTools : MonoBehaviour
{
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuitGame();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            LoadScene("MainMenu");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public static void GoToMainMenu()
    {
        LoadScene("MainMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}