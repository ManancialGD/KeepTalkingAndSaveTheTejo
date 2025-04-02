using UnityEngine;

public class DisplaysControler : MonoBehaviour
{
    private void Start()
    {
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
            Debug.Log($"Display {i} activated.");
        }
    }
}
