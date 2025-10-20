using TMPro;
using UnityEngine;

public class WinLoseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionCountText;

    public void UpdateText(int questionCount)
    {
        questionCountText.text = $"Você gahnou com {questionCount} Perguntas!";
    }    
}
