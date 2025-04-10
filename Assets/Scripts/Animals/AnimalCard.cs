using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalCard : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private Animal animal;

    private bool isActive = true;
    public bool IsActive { get { return isActive; } }

    private Animator anim;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image animalImage;

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
            if (nameText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found on the AnimalCard GameObject.");
            }
        }
        if (animalImage == null)
        {
            Image[] childImages = GetComponentsInChildren<Image>();
            animalImage = childImages.FirstOrDefault(i => i.name == "AnimalImage") ?? null;
            if (animalImage == null)
            {
                Debug.LogError("Image component not found on the AnimalCard GameObject.");
            }
        }
        if (anim == null)
        {
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogError("Animator component not found on the AnimalCard GameObject.");
            }
        }
    }

    public void Select()
    {
        anim.SetTrigger("Select");
    }
    public void Deselect()
    {
        anim.SetTrigger("Deselect");
    }
    public void ToggleDiscart()
    {
        if (isActive)
            anim.SetTrigger("Discart");
        else
            anim.SetTrigger("Undiscart");

        isActive = !isActive;
    }
}
