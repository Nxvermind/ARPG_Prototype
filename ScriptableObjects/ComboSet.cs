using UnityEngine;

public abstract class ComboSet : ScriptableObject
{
    public abstract AttackNode FirstLightAttack { get; }
    public abstract AttackNode FirstHeavyAttack { get; }
    public abstract AttackNode ChargeAttack { get; }
    public abstract AttackNode AirAttack { get; }
    public abstract AttackNode ParryAttack { get; }
}
