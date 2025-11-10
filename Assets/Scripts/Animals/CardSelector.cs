using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

using Random = UnityEngine.Random;

public class CardSelector : MonoBehaviour
{
    [SerializeField]
    private ArduinoInput arduinoInput;
    [SerializeField] private AnimalCard[] animalCards;
    [SerializeField] private Animal[] allAnimals;
    public Animal ThisAnimal { get; private set; }

    [SerializeField] private InputActionReference primaryActionReference;
    [SerializeField] private InputActionReference secondaryActionReference;
    [SerializeField] private WinLoseUI winLoseUI;
    private bool gameEnded;

    [SerializeField] private Animator QuestionCardPanel;
    private Animator[] questionCards;

    private int questionCardSelectedID;
    public event Action Win;

    // Raw data loaded from JSON
    private Dictionary<string, Dictionary<string, string>> questionsData;

    [SerializeField] private TextMeshProUGUI previusText;

    private int questionCount = 0;

    // Pool of questions that are not answered and not currently assigned to visible cards.
    private List<string> availableQuestions = new List<string>();

    // Normalized answered set for quick comparisons
    private HashSet<string> answeredNormalized = new HashSet<string>();

    private void Start()
    {
        OnValidate();

        Win += OnWin;

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

        // pick random animal
        ThisAnimal = allAnimals.OrderBy(_ => Random.value).FirstOrDefault();

        // initialize UI and question pool
        UpdateQuestions();
        if (QuestionCardPanel != null)
            QuestionCardPanel.SetTrigger("Show");

        ValidateQuestionData(questionsData);

        arduinoInput.Button1Down += DoPrimaryAction;
        arduinoInput.Button2Down += DoSecondaryAction;
    }

    private void UpdateQuestions()
    {
        if (QuestionCardPanel == null)
            return;

        // collect card animators (exclude panel root)
        questionCards = QuestionCardPanel.GetComponentsInChildren<Animator>(true)
            .Where(anim => anim.gameObject != QuestionCardPanel.gameObject)
            .ToArray();

        var questionTexts = questionCards
            .Select(card => card.GetComponentInChildren<TextMeshProUGUI>(true))
            .ToArray();

        // Build fresh pool from source, excluding already answered questions
        var pool = questionsData.Keys
            .Where(k => !answeredNormalized.Contains(Normalize(k)))
            .ToList();

        // Shuffle pool (Fisher-Yates)
        Shuffle(pool);

        // Assign first N questions to visible cards
        List<string> usedQuestions = new List<string>();
        for (int i = 0; i < questionTexts.Length; i++)
        {
            if (i < pool.Count)
            {
                questionTexts[i].text = pool[i];
                usedQuestions.Add(pool[i]);
            }
            else
            {
                questionTexts[i].text = "";
            }
        }

        // Set availableQuestions to what's left in the pool after assignment
        availableQuestions = pool.Skip(usedQuestions.Count).ToList();

        questionCardSelectedID = 0;

        foreach (var card in questionCards)
            card.SetBool("isSelected", false);

        if (questionCards.Length > 0)
            questionCards[0].SetBool("isSelected", true);
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
        DoPrimaryAction();
    }

    private void DoPrimaryAction()
    {
        if (gameEnded)
        {
            SceneTools.GoToMainMenu();
            return;
        }
        NextQuestionCard();
    }

    private void OnSecondaryAction(InputAction.CallbackContext context)
    {
        DoSecondaryAction();
    }

    private void DoSecondaryAction()
    {
        if (gameEnded)
        {
            SceneTools.GoToMainMenu();
            return;
        }

        SelectQuestionCards();
        CheckWinConditions();
    }

    private void SelectQuestionCards()
    {
        if (questionCards == null || questionCards.Length == 0)
            return;

        TextMeshProUGUI questionText = questionCards[questionCardSelectedID].GetComponentInChildren<TextMeshProUGUI>();
        if (questionText == null)
            return;

        string displayedQuestion = questionText.text;
        if (string.IsNullOrEmpty(displayedQuestion) || ThisAnimal == null || questionsData == null)
            return;

        Debug.Log($"Animal : {ThisAnimal.Name}, Question: {displayedQuestion}");

        // Find the exact entry in questionsData ignoring case/whitespace
        string normalizedDisplayed = Normalize(displayedQuestion);
        string actualKey = questionsData.Keys.FirstOrDefault(k => Normalize(k) == normalizedDisplayed);

        if (actualKey == null)
        {
            Debug.LogError($"No data found for question '{displayedQuestion}'.");
            return;
        }

        var animalAnswers = questionsData[actualKey];

        if (!animalAnswers.TryGetValue(ThisAnimal.Name, out var correctAnswer))
        {
            Debug.LogError($"No answer found for animal '{ThisAnimal.Name}' in question '{actualKey}'.");
            return;
        }

        // Show answer and count it
        previusText.text = $"{actualKey}       {correctAnswer}";
        questionCount++;

        // Mark answered (normalized) immediately
        if (!answeredNormalized.Contains(normalizedDisplayed))
            answeredNormalized.Add(normalizedDisplayed);

        // Remove the answered question from availableQuestions if present
        availableQuestions.RemoveAll(q => Normalize(q) == normalizedDisplayed);

        // Apply elimination logic to animal cards
        foreach (AnimalCard animalCard in animalCards)
        {
            if (animalAnswers.TryGetValue(animalCard.Animal.Name, out var animalAnswer))
            {
                if (animalAnswer != correctAnswer)
                {
                    animalCard.SetDiscart();
                }
            }
        }

        // Prepare set of currently visible questions (normalized)
        var currentVisibleNormalized = questionCards
            .Select(c => c.GetComponentInChildren<TextMeshProUGUI>())
            .Where(t => t != null && !string.IsNullOrEmpty(t.text))
            .Select(t => Normalize(t.text))
            .ToHashSet();

        // Try to pick replacement from availableQuestions first (ensures we don't pick something that was just assigned)
        string replacement = PickRandomQuestionExcluding(availableQuestions, currentVisibleNormalized, answeredNormalized);

        // If nothing found (availableQuestions empty or all excluded), rebuild a fallback pool from questionsData
        if (replacement == null)
        {
            var fallback = questionsData.Keys
                .Where(k => !answeredNormalized.Contains(Normalize(k)) && !currentVisibleNormalized.Contains(Normalize(k)))
                .ToList();

            if (fallback.Count > 0)
            {
                Shuffle(fallback);
                replacement = fallback[Random.Range(0, fallback.Count)];
            }
        }

        if (!string.IsNullOrEmpty(replacement))
        {
            // assign and remove from master availableQuestions if present
            questionText.text = replacement;
            availableQuestions.RemoveAll(q => Normalize(q) == Normalize(replacement));
        }
        else
        {
            questionText.text = "";
            Debug.Log("No more available unique questions to select.");
        }
    }

    private string PickRandomQuestionExcluding(List<string> pool, HashSet<string> excludeVisibleNormalized, HashSet<string> answeredSetNormalized)
    {
        if (pool == null || pool.Count == 0)
            return null;

        // Filter pool by exclusion sets (normalized)
        var candidates = pool.Where(q => !excludeVisibleNormalized.Contains(Normalize(q)) && !answeredSetNormalized.Contains(Normalize(q))).ToList();
        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
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

    private void CheckWinConditions()
    {
        Animal selectedAnimal = null;
        uint discardedAnimalCount = 0;

        foreach (AnimalCard animalCard in animalCards)
        {
            if (animalCard.IsActive)
                selectedAnimal = animalCard.Animal;
            else
                discardedAnimalCount++;
        }

        if (discardedAnimalCount == animalCards.Length - 1)
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
        winLoseUI.UpdateText(questionCount);
        gameEnded = true;
        QuestionCardPanel.SetTrigger("Hide");
    }

    private void OnValidate()
    {
        if (animalCards == null || animalCards.Length == 0)
        {
            animalCards = GetComponentsInChildren<AnimalCard>();
        }
    }

    // Simple in-place Fisher-Yates shuffle
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private string Normalize(string s)
    {
        return (s ?? "").Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Verifica se o ficheiro de perguntas permite distinguir todos os animais.
    /// Regista no console pares de animais que têm respostas idênticas.
    /// </summary>
    private void ValidateQuestionData(Dictionary<string, Dictionary<string, string>> data)
    {
        if (data == null || data.Count == 0)
        {
            Debug.LogError("❌ [Validation] Nenhuma pergunta carregada.");
            return;
        }

        // Extrair todos os nomes de animais
        var allAnimals = data.Values
            .SelectMany(dict => dict.Keys)
            .Distinct()
            .OrderBy(n => n)
            .ToList();

        int identicalPairs = 0;

        // Comparar todos os pares de animais
        for (int i = 0; i < allAnimals.Count; i++)
        {
            for (int j = i + 1; j < allAnimals.Count; j++)
            {
                string animalA = allAnimals[i];
                string animalB = allAnimals[j];

                bool allEqual = true;

                foreach (var question in data.Keys)
                {
                    if (!data[question].TryGetValue(animalA, out var answerA))
                    {
                        Debug.LogWarning($"⚠️ [Validation] Pergunta '{question}' não tem resposta para '{animalA}'.");
                        allEqual = false;
                        continue;
                    }

                    if (!data[question].TryGetValue(animalB, out var answerB))
                    {
                        Debug.LogWarning($"⚠️ [Validation] Pergunta '{question}' não tem resposta para '{animalB}'.");
                        allEqual = false;
                        continue;
                    }

                    if (answerA != answerB)
                    {
                        allEqual = false;
                        break;
                    }
                }

                if (allEqual)
                {
                    identicalPairs++;
                    Debug.LogError($"❌ [Validation] Animais indistinguíveis: '{animalA}' e '{animalB}' têm respostas idênticas.");
                }
            }
        }

        if (identicalPairs == 0)
            Debug.Log($"✅ [Validation] Todos os {allAnimals.Count} animais são distinguíveis com {data.Count} perguntas.");
        else
            Debug.LogError($"⚠️ [Validation] Encontrados {identicalPairs} pares indistinguíveis.");
    }

}
