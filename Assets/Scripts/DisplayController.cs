using UnityEngine;

public class DisplaysController : MonoBehaviour
{
    private void Start()
    {
        if (Display.displays != null && Display.displays.Length == 2)
        {
            Display.displays[0].Activate();
            Display.displays[1].Activate();
        }
    }
}
