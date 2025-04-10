using UnityEngine;


[CreateAssetMenu(fileName = "Symbol", menuName = "Scriptable Objects/Symbol")]
public class Symbology : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
}
