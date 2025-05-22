using TMPro;
using UnityEngine;

public class WinLoseUI : MonoBehaviour
{
    [SerializeField] string winText = "Vitória";
    [SerializeField] string loseText = "Derrota";

    [SerializeField] private TextMeshProUGUI winLoseText;
    [SerializeField] private AnimalCard animalCard;

    private void Awake()
    {
        if (winLoseText == null)
        {
            Debug.LogError("TextMeshProUGUI component not assigned in " + gameObject.name);
        }
        else winLoseText.text = "";
    }

    public void ShowWin(Animal oponentAnimal)
    {
        winLoseText.text = winText;
        animalCard.Animal = oponentAnimal;
        animalCard.UpdateSymbols();
    }

    public void ShowLose(Animal oponentAnimal)
    {
        winLoseText.text = loseText;
        animalCard.Animal = oponentAnimal;
        animalCard.UpdateSymbols();
    }
}
