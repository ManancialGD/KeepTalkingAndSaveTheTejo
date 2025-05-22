using UnityEngine;

public class StartUpManager : MonoBehaviour
{
    private bool gameEnded;

    public void StartGame()
    {
        int currentLevel;

        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 1);
            currentLevel = 1;
        }
        else
            currentLevel = PlayerPrefs.GetInt("Level");


        switch (currentLevel)
        {
            case 1:
                SceneTools.LoadScene("Level1");
                break;
            case 2:
                SceneTools.LoadScene("Level2");
                break;
            case 3:
                SceneTools.LoadScene("Level3");
                break;
            case 4:
                SceneTools.LoadScene("Level4");
                break;
            default:
                SceneTools.LoadScene("Level1");
                break;
        }
    }

    public void StartEspecificLevel(string level)
    {
        SceneTools.LoadScene(level);
    }

    private void Start()
    {
        AnimalSelector[] animalSelectors = FindObjectsByType<AnimalSelector>(FindObjectsSortMode.None);

        foreach (AnimalSelector animalSelector in animalSelectors)
            animalSelector.Win += OnEndGame;
    }


    private void OnEndGame()
    {
        if (!gameEnded)
        {
            gameEnded = !gameEnded;
            EndGame();
        }
    }

    public void EndGame()
    {
        int nextLevel = PlayerPrefs.GetInt("Level") + 1;
        if (nextLevel > 4)
            nextLevel = 1;
        PlayerPrefs.SetInt("Level", nextLevel);
    }
}
