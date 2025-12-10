using UnityEngine;

[CreateAssetMenu(menuName = "Combo/WeaponComboSet")]
public class WeaponComboSet : ComboSet
{
    public AttackNode firstLightAttackNode;

    public AttackNode firstHeavyAttackNode;

    public AttackNode chargeAttackNode;

    public AttackNode airAttackNode;

    public AttackNode parryAttackNode;

    public override AttackNode FirstLightAttack => firstLightAttackNode;

    public override AttackNode FirstHeavyAttack => firstHeavyAttackNode;

    public override AttackNode ChargeAttack => chargeAttackNode;

    public override AttackNode AirAttack => airAttackNode;
    public override AttackNode ParryAttack => parryAttackNode;
}
