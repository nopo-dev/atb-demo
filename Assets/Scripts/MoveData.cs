using UnityEngine;

public enum Target
{
    Ally, Enemy, Self
}

public enum TargetNumber
{
    Single, All
}

public enum Effect
{
    Damage, Heal, Block
}


[CreateAssetMenu(fileName = "MoveData", menuName = "Scriptable Objects/MoveData")]
public class MoveData : ScriptableObject
{
    [SerializeField] public string MoveName;
    [SerializeField] public Effect MoveEffect;
    [SerializeField] public int MoveValue;
    [SerializeField] public Target MoveTarget;
    [SerializeField] public TargetNumber MoveTargetNumber;
}
