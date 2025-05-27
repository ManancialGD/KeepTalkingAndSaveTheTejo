using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

using Random = UnityEngine.Random;

public class AnimalSelector : MonoBehaviour
{
    [SerializeField] private AnimalCard[] animalsCards;
    [SerializeField] private uint currentCardID;
    [SerializeField] private AnimalSelector other;
    public Animal ThisAnimal { get; set; }
    public MyAnimalCardDisplay myAnimalDisplay;

    [SerializeField] private InputActionReference primaryActionReference;
    [SerializeField] private InputActionReference secondaryActionReference;
    [SerializeField] private WinLoseUI winLoseUI;
    private bool gameEnded;

    [SerializeField] private bool useTimer;
    [SerializeField] private bool myTurn;
    [SerializeField] private float timer;

    public event Action Win;

    private IEnumerator Start()
    {
        OnValidate();

        other.Win += OnLose;
        Win += OnWin;

        currentCardID = 0;

        gameEnded = false;
        
        yield return null;

        animalsCards[currentCardID].Select();
    }

    private void Update()
    {
        if (myTurn && useTimer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                OnLose();
                other.OnWin();
            }
        }
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
        if (gameEnded)
        {
            SceneTools.GoToMainMenu();
        }
        if (myTurn)
        {
            NextCard();
        }
    }
    private void OnSecondaryAction(InputAction.CallbackContext context)
    {
        if (gameEnded)
        {
            SceneTools.GoToMainMenu();
        }
        if (myTurn)
        {
            ToggleCard();
        }
        CheckWinConditions();
    }

    private void NextCard()
    {
        if (currentCardID < animalsCards.Length)
            animalsCards[currentCardID].Deselect();
        currentCardID++;

        if (currentCardID > animalsCards.Length)
            currentCardID = 0;

        if (currentCardID != animalsCards.Length)
            animalsCards[currentCardID].Select();
    }

    private void ToggleCard()
    {
        if (currentCardID < animalsCards.Length)
        {
            animalsCards[currentCardID].ToggleDiscart();
        }
        
        if (currentCardID == animalsCards.Length)
        {
            EndTurn();
        }
    }

    private void EndTurn()
    {
        myTurn = false;
        other.StartTurn();
    }

    public void StartTurn()
    {
        myTurn = true;
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
                Win?.Invoke();
            }
        }
    }

    private void OnWin()
    {
        winLoseUI.gameObject.SetActive(true);
        winLoseUI.ShowWin(other.ThisAnimal);
        gameEnded = true;
    }

    private void OnLose()
    {
        winLoseUI.gameObject.SetActive(true);
        winLoseUI.ShowLose(other.ThisAnimal);
        gameEnded = true;
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
