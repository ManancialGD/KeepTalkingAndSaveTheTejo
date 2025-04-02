using System;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "TwoScreensTest";
    [SerializeField] private Button[] startButtons;
    [SerializeField] private TextMeshProUGUI[] startButtonCountText;

    private SceneTools sceneTools;
    private bool button1Pressed;
    private bool button2Pressed;
    private int buttonsPressed;

    private void Awake()
    {
        button1Pressed = false;
        button2Pressed = false;
        buttonsPressed = 0;
    }

    private void Start()
    {
        if (sceneTools == null)
        {
            SceneTools sceneSceneTools = FindAnyObjectByType<SceneTools>();
            if (sceneSceneTools == null)
            {
                GameObject obj = new("SceneTools");
                sceneSceneTools = obj.AddComponent<SceneTools>();
            }
            sceneTools = sceneSceneTools;
        }

        OnValidate();
    }

    private void OnEnable()
    {
        if (startButtons != null && startButtons.Length == 2)
        {
            startButtons[0].onClick.AddListener(() => HandleButtonPress(ref button1Pressed));
            startButtons[1].onClick.AddListener(() => HandleButtonPress(ref button2Pressed));
        }
        else
        {
            Debug.LogError("Couldn't find the start buttons. Set it manually in the inspector.", this);
        }
    }

    private void OnDisable()
    {
        if (startButtons != null && startButtons.Length == 2)
        {
            startButtons[0].onClick.RemoveListener(() => HandleButtonPress(ref button1Pressed));
            startButtons[1].onClick.RemoveListener(() => HandleButtonPress(ref button2Pressed));
        }
    }

    private void HandleButtonPress(ref bool buttonPressed)
    {
        if (buttonPressed)
        {
            buttonPressed = false;
            buttonsPressed--;
        }
        else
        {
            buttonPressed = true;
            buttonsPressed++;
            if (buttonsPressed == 2)
            {
                sceneTools.LoadScene(sceneToLoad);
                return;
            }
        }

        string buttonText = buttonsPressed != 0 ? $"({buttonsPressed}/2)" : "";
        Array.ForEach(startButtonCountText, text => text.text = buttonText);
    }

    private void OnValidate()
    {
        if (startButtons == null)
        {
            var buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

            if (buttons.Length == 2)
            {
                startButtons = buttons;

            }
            else
            {
                Debug.LogError("Couldn't find the start buttons. Set it manually in the inspector.", this);
            }
        }
        else if (startButtons.Length != 2)
        {
            Debug.LogError("The start buttons array must have exactly 2 elements.", this);
        }

        bool countTextCheck = startButtonCountText == null || startButtonCountText.Length == 0;
        bool startButtonsCheck = startButtons != null || startButtons.Length == 2;

        if (countTextCheck && startButtonsCheck)
        {
            TextMeshProUGUI text1 = startButtons[0].GetComponentsInChildren<TextMeshProUGUI>().Where(t => t.text == "").FirstOrDefault();
            TextMeshProUGUI text2 = startButtons[1].GetComponentsInChildren<TextMeshProUGUI>().Where(t => t.text == "").FirstOrDefault();

            if (text1 != null && text2 != null)
            {
                startButtonCountText = new TextMeshProUGUI[2]
                {
                    text1,
                    text2
                };
            }
        }
    }
}
