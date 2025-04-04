using UnityEngine;

public class AnimalSelection : MonoBehaviour
{
    [SerializeField] private AnimalCard[] animals;
    [SerializeField] private uint currentCardID;

    private void Awake()
    {
        animals = GetComponentsInChildren<AnimalCard>();
        currentCardID = 0;
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
}
