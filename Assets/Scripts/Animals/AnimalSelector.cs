using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimalSelector : MonoBehaviour
{
    [SerializeField] private AnimalCard[] animals;
    [SerializeField] private uint currentCardID;
    [SerializeField] private AnimalSelector other;
    public Animal ThisAnimal { get; private set; }
    [SerializeField] private MyAnimalCardDisplay myAnimalDisplay;
    private Animal[] allAnimals;

    [SerializeField] private InputActionReference primaryActionReference;
    [SerializeField] private InputActionReference secondaryActionReference;

    private void Awake()
    {
        OnValidate();

        allAnimals = Resources.LoadAll<Animal>("Animals").ToArray();
        if (allAnimals != null && allAnimals.Length > 0)
        {
            if (other.ThisAnimal == null)
            {
                ThisAnimal = allAnimals[Random.Range(0, allAnimals.Length)];
            }
            else
            {
                var animalsWithoutOther = allAnimals.Where(a => a != other.ThisAnimal).ToArray();
                ThisAnimal = animalsWithoutOther[Random.Range(0, animalsWithoutOther.Length)];
            }
        }

        myAnimalDisplay.Animal = ThisAnimal;

        currentCardID = 0;
    }
    private void OnEnable()
    {
        if (primaryActionReference != null)
            primaryActionReference.action.performed += OnPrimaryAction;
        if (secondaryActionReference != null)
            secondaryActionReference.action.performed += OnSecondaryAction;
    }
    private void OnDisable()
    {
        if (primaryActionReference != null)
            primaryActionReference.action.performed -= OnPrimaryAction;
        if (secondaryActionReference != null)
            secondaryActionReference.action.performed -= OnSecondaryAction;
    }

    private void OnPrimaryAction(InputAction.CallbackContext context)
    {
        NextCard();

    }
    private void OnSecondaryAction(InputAction.CallbackContext context)
    {
        ToggleCard();
    }

    private void Start()
    {
        animals[currentCardID].Select();
    }

    private void NextCard()
    {
        animals[currentCardID].Deselect();
        currentCardID++;

        if (currentCardID >= animals.Length)
            currentCardID = 0;

        animals[currentCardID].Select();
    }

    private void ToggleCard()
    {
        animals[currentCardID].ToggleDiscart();
    }

    private void OnValidate()
    {
        if (animals == null || animals.Length == 0)
        {
            animals = GetComponentsInChildren<AnimalCard>();
        }
        if (other == null)
        {
            other = FindObjectsByType<AnimalSelector>(FindObjectsSortMode.None).FirstOrDefault(x => x != this);
        }
    }
}
