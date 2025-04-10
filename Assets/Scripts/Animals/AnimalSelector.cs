using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimalSelector : MonoBehaviour
{
    [SerializeField] private AnimalCard[] animalsCards;
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
        animalsCards[currentCardID].Select();
    }

    private void NextCard()
    {
        animalsCards[currentCardID].Deselect();
        currentCardID++;

        if (currentCardID >= animalsCards.Length)
            currentCardID = 0;

        animalsCards[currentCardID].Select();
    }

    private void ToggleCard()
    {
        animalsCards[currentCardID].ToggleDiscart();
    }

    private void CheckWinConditions()
    {
        Animal selectedAnimal = null;
        uint discardedAnimalCount = 0;

        foreach (AnimalCard animalCard in animalsCards)
        {
            if (animalCard.IsActive)
                selectedAnimal = animalCard.Animal;
            else
                discardedAnimalCount++;
        }

        if (discardedAnimalCount == animalsCards.Length - 1)
        {
            if (selectedAnimal == other.ThisAnimal)
            {
                Win();
            }
            else
            {
                Lose();
            }
        }
    }

    private void Win()
    {
        Debug.Log("You got it right!");
    }

    private void Lose()
    {
        Debug.Log("You got it wrong!");
    }

    private void OnValidate()
    {
        if (animalsCards == null || animalsCards.Length == 0)
        {
            animalsCards = GetComponentsInChildren<AnimalCard>();
        }
        if (other == null)
        {
            other = FindObjectsByType<AnimalSelector>(FindObjectsSortMode.None).FirstOrDefault(x => x != this);
        }
    }
}
