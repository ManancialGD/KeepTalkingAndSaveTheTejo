using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalCard : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private Animal animal;

    private Animator anim;

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
            if (nameText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found on the AnimalCard GameObject.");
            }
        }
        if (animalImage == null)
        {
            animalImage = GetComponentInChildren<Image>();
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
    public void Discart()
    {
        anim.SetTrigger("Discart");
    }

    public void Undiscart()
    {
        anim.SetTrigger("Undiscart");
    }
}
