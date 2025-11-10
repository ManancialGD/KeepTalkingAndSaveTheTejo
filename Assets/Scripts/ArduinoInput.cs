using System;
using UnityEngine;

public class ArduinoInput : MonoBehaviour
{
    [SerializeField]

    public SerialController serialController;

    public event Action Button1Down;
    public event Action Button2Down;

    public void OnMessageArrived(string message)
    {

        if (message == null)
            return;

        if (message.Equals("B1_PRESSED"))
        {
            Debug.Log("Button 1 pressed!");
            Button1Down?.Invoke();
        }
        else if (message.Equals("B1_RELEASED"))
            Debug.Log("Button 1 released!");
        else if (message.Equals("B2_PRESSED"))
        {
            Debug.Log("Button 2 pressed!");
            Button2Down?.Invoke();
        }
        else if (message.Equals("B2_RELEASED"))
            Debug.Log("Button 2 released!");
    }
}
