using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.InputSystem;

using Random = UnityEngine.Random;

public class AnimalSelector : MonoBehaviour
{
    [SerializeField] private AnimalCard[] animalsCards;
    [SerializeField] private uint currentCardID;
    public Animal ThisAnimal { get; set; }
    public MyAnimalCardDisplay myAnimalDisplay;

    [SerializeField] private InputActionReference primaryActionReference;
    [SerializeField] private InputActionReference secondaryActionReference;
    [SerializeField] private WinLoseUI winLoseUI;
    private bool gameEnded;
    [SerializeField] private Animator nextButton;

    [SerializeField] private Animator QuestionCardPanel;
    private Animator[] questionCards;

    private bool selectingQuestionCards;
    private int questionCardSelectedID;
    public event Action Win;

    private Dictionary<string, Dictionary<string, string>> questionsData;

    [SerializeField] private TextMeshProUGUI previusText;

    private IEnumerator Start()
    {
        OnValidate();

        Win += OnWin;

        currentCardID = 0;

        gameEnded = false;

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "questions.json");
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            questionsData = JsonUtility.FromJson<SerializableQuestionsData>(json).ToDictionary();
        }
        else
        {
            Debug.LogWarning("Questions JSON not found at: " + path);
            questionsData = new Dictionary<string, Dictionary<string, string>>();
        }

        yield return null;

        animalsCards[currentCardID].Select();


    }

    private void StartSelectingQuestionCards()
    {
        if (QuestionCardPanel != null)
        {
            selectingQuestionCards = true;

            QuestionCardPanel.SetTrigger("Show");
            questionCards = QuestionCardPanel.GetComponentsInChildren<Animator>()
                .Where(anim => anim.gameObject != QuestionCardPanel.gameObject)
                .ToArray();

            var questionTexts = questionCards.Select(card => card.GetComponentInChildren<TextMeshProUGUI>()).ToArray();
            var availableQuestions = questionsData.Keys.OrderBy(_ => Random.value).Take(questionCards.Length).ToList();

            for (int i = 0; i < questionTexts.Length; i++)
            {
                if (i < availableQuestions.Count)
                    questionTexts[i].text = availableQuestions[i];
                else
                    questionTexts[i].text = "";
            }
            questionCardSelectedID = 0;
            
            foreach (var card in questionCards)
            {
                card.SetBool("isSelected", false);
            }
            questionCards[0].SetBool("isSelected", true);
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
        if (!selectingQuestionCards)
        {
            NextCard();
        }
        else
        {
            NextQuestionCard();
        }
    }
    private void OnSecondaryAction(InputAction.CallbackContext context)
    {
        if (gameEnded)
        {
            SceneTools.GoToMainMenu();
        }
        if (!selectingQuestionCards)
        {
            ToggleCard();
            CheckWinConditions();
        }
        else
        {
            SelectQuestionCards();
        }
    }

    private void SelectQuestionCards()
    {
        if (questionCards == null || questionCards.Length == 0)
            return;

        TextMeshProUGUI questionText = questionCards[questionCardSelectedID].GetComponentInChildren<TextMeshProUGUI>();

        string question = questionText != null ? questionText.text : null;

        if (!string.IsNullOrEmpty(question) && ThisAnimal != null && questionsData != null)
        {
            Debug.Log($"Animal : {ThisAnimal.Name}, Question: {question}");
            if (questionsData.TryGetValue(question, out var animalNames))
            {
                if (animalNames.TryGetValue(ThisAnimal.Name, out var answer))
                {
                    TextMeshProUGUI[] cards = questionCards.Select(card => card.GetComponentInChildren<TextMeshProUGUI>()).ToArray();

                    var cardTexts = cards.Select(c => c.text).ToList();
                    string[] questions = questionsData.Keys.Where(q => !cardTexts.Contains(q)).ToArray();
                    questionText.text = questions[Random.Range(0, questions.Length)];
                    previusText.text = $"{question}       {answer}";
                    Debug.Log(answer);
                }
                else
                {
                    Debug.LogError("No answer found.");
                }
            }
            else
            {
                Debug.LogError("No data for this animal.");
            }
        }

        selectingQuestionCards = false;
        QuestionCardPanel.SetTrigger("Hide");
    }

    private void NextQuestionCard()
    {
        if (questionCards == null || questionCards.Length == 0)
            return;

        questionCards[questionCardSelectedID].SetBool("isSelected", false);
        questionCardSelectedID++;
        if (questionCardSelectedID >= questionCards.Length)
            questionCardSelectedID = 0;
        questionCards[questionCardSelectedID].SetBool("isSelected", true);
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

        if (currentCardID == animalsCards.Length)
        {
            nextButton.SetBool("isSelected", true);
        }
        else
            nextButton.SetBool("isSelected", false);
    }

    private void ToggleCard()
    {
        if (currentCardID < animalsCards.Length)
        {
            animalsCards[currentCardID].ToggleDiscart();
        }

        if (currentCardID == animalsCards.Length)
        {
            StartSelectingQuestionCards();
            nextButton.SetBool("isSelected", false);
        }
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
            if (selectedAnimal == ThisAnimal)
            {
                Win?.Invoke();
            }
        }
    }

    private void OnWin()
    {
        winLoseUI.gameObject.SetActive(true);
        gameEnded = true;
    }

    private void OnLose()
    {
        winLoseUI.gameObject.SetActive(true);
        gameEnded = true;
    }

    private void OnValidate()
    {
        if (animalsCards == null || animalsCards.Length == 0)
        {
            animalsCards = GetComponentsInChildren<AnimalCard>();
        }
    }
}
