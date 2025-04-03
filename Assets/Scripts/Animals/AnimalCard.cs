using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalCard : MonoBehaviour, ISerializationCallbackReceiver
{
    public Animal animal;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image animalImage;
    
    private void Awake()
    {
        OnValidate();
    }

    public void OnBeforeSerialize()
    {
        if (animal != null)
        {
            if (nameText != null)
            {
                nameText.text = animal.Name;
            }
            if (animalImage != null)
            {
                animalImage.sprite = animal.Image;
            }
        }
    }

    public void OnAfterDeserialize() { }

    private void OnValidate()
    {
        if (nameText == null)
        {
            nameText = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (animalImage == null)
        {
            animalImage = GetComponentInChildren<Image>();
        }
    }
}
