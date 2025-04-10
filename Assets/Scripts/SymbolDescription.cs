using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SymbolDescription : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private Symbology symbol;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image symbolImage;

    public Symbology Symbol
    {
        get => symbol;
        set
        {
            symbol = value;
            OnBeforeSerialize();
        }
    }

    private void Awake()
    {
        OnBeforeSerialize();
    }

    public void OnBeforeSerialize()
    {
        if (symbol != null)
        {
            if (nameText != null)
            {
                nameText.text = symbol.Name ?? "";
            }
            if (symbolImage != null)
            {
                symbolImage.sprite = symbol.Image != null ? symbol.Image : null;
            }
        }
    }

    public void OnAfterDeserialize() { }
}
