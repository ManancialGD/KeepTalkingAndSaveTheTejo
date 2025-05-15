using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuNavigator : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference selectAction;

    [SerializeField] private Menu activeMenu;

    private int selectedButtonIndex;

    private void Start()
    {
        if (activeMenu == null)
        {
            Debug.LogError("Active menu is not assigned.");
            return;
        }

        selectedButtonIndex = 0;

        if (activeMenu.Buttons.Length > 0 && activeMenu.Buttons != null)
        {
            Array.ForEach(activeMenu.Buttons, button => button.SetSelection(false));
            activeMenu.Buttons[0].SetSelection(true);
        }
    }

    public void SetActiveMenu(Menu menu)
    {
        activeMenu = menu;
        selectedButtonIndex = 0;

        if (activeMenu.Buttons.Length > 0 && activeMenu.Buttons != null)
        {
            Array.ForEach(activeMenu.Buttons, button => button.SetSelection(false));
            activeMenu.Buttons[0].SetSelection(true);
        }
    }

    private void OnEnable()
    {
        moveAction.action.performed += OnMovePerformed;
        selectAction.action.performed += OnSelectPerformed;
    }
    private void OnDisable()
    {
        moveAction.action.performed -= OnMovePerformed;
        selectAction.action.performed -= OnSelectPerformed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        selectedButtonIndex = (selectedButtonIndex + 1) % activeMenu.Buttons.Length;
        if (activeMenu.Buttons != null && activeMenu.Buttons.Length > 0)
        {
            Array.ForEach(activeMenu.Buttons, button => button.SetSelection(false));
            activeMenu.Buttons[selectedButtonIndex].SetSelection(true);
        }
    }

    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            activeMenu.Buttons[selectedButtonIndex].OnClick();
        }
    }

}
