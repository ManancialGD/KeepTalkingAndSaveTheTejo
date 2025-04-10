using UnityEngine;

[CreateAssetMenu(fileName = "Animal", menuName = "Scriptable Objects/Animal")]
public class Animal : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
    [SerializeField] private string[] constraints;
    public string[] Constraints { get => (string[])constraints.Clone(); }
    [SerializeField] private Symbology[] symbols;
    public Symbology[] Symbols { get => (Symbology[])symbols.Clone(); }
}
