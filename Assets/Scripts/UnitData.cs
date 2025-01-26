using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    [SerializeField] public int MaxHealth = 20;
    [SerializeField] public int Attack = 5;
    [SerializeField] public float MaxActionValue = 10f;
    [SerializeField] public float MaxStamina = 10f;
    [SerializeField] public float Speed = 1f;
    [SerializeField] public float Regen = 1f;
}