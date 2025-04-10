using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MyAnimalCardDisplay : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private Animal animal;

    [SerializeField] private Image animalImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI constraintsText;

    public Animal Animal
    {
        get => animal;
        set
        {
            animal = value;
            OnBeforeSerialize();
        }
    }

    private void Awake()
    {
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
            if (constraintsText != null)
            {
                SetupConstrants();
            }
        }
    }

    private void SetupConstrants()
    {
        if (animal.Constraints != null && animal.Constraints.Length > 0)
        {
            constraintsText.text = string.Join("\n", animal.Constraints);
        }
        else
        {
            constraintsText.text = "Nenhum";
        }
    }

    public void OnAfterDeserialize() { }
}
