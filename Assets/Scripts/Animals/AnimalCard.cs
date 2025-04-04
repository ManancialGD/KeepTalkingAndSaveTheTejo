using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalCard : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private Animal animal;

    public Animal Animal
    {
        get => animal;
        set
        {
            animal = value;
            OnBeforeSerialize();
        }
    }

    private TextMeshProUGUI nameText;
    private Image animalImage;

    private void Awake()
    {
        OnValidate();
        OnBeforeSerialize();
    }

    public void OnBeforeSerialize()
    {
        if (animal != null)
        {
            if (nameText != null)
            {
                nameText.text = animal.Name ?? "";
            }
            if (animalImage != null)
            {
                animalImage.sprite = animal.Image != null ? animal.Image : null;
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
