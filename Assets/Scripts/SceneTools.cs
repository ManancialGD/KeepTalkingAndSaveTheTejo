using Unity.VisualScripting;
using UnityEngine;

public class SceneTools : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
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
        if (Input.GetKeyDown(KeyCode.S))
        {
            LoadScene("Prototype");
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}