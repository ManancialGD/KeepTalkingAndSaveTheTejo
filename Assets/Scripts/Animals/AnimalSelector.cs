using System.Linq;
using UnityEngine;

public class AnimalSelector : MonoBehaviour
{
    [SerializeField] private AnimalCard[] animals;
    [SerializeField] private uint currentCardID;
    [SerializeField] private AnimalSelector other;
    public Animal ThisAnimal { get; private set; }
    [SerializeField] private MyAnimalCardDisplay myAnimalDisplay;
    private Animal[] allAnimals;

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

    private void Start()
    {
        animals[currentCardID].Select();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            NextCard();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ToggleCard();
        }
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
