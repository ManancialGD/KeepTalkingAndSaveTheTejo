using UnityEngine;

public class DisplaysController : MonoBehaviour
{
    private void Start()
    {
        if (Display.displays != null && Display.displays.Length == 2)
        {
            if (Display.displays[0].active == false)
            {
                Display.displays[0].Activate();
            }
            if (Display.displays[1].active == false)
            {
                Display.displays[1].Activate();
            }
        }
    }
}
