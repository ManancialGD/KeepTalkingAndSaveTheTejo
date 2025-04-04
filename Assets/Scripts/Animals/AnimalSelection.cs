using UnityEngine;

public class AnimalSelection : MonoBehaviour
{
    [SerializeField] private AnimalCard[] animals;
    [SerializeField] private uint currentCardID;

    private void Awake()
    {
        OnValidate();
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
        if (animals[currentCardID].IsActive)
        {
            animals[currentCardID].Discart();
        }
        else animals[currentCardID].Undiscart();

        animals[currentCardID].IsActive = !animals[currentCardID].IsActive;
    }

    private void OnValidate()
    {
        if (animals == null || animals.Length == 0)
        {
            animals = GetComponentsInChildren<AnimalCard>();
        }
    }
}
