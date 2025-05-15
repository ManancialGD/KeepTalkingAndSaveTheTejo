using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private MenuButton[] menuButtons;
    public MenuButton[] Buttons => (MenuButton[])menuButtons.Clone();
}