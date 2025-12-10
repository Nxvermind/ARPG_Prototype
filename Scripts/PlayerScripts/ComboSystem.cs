using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    public ComboSet weaponComboSet;
    public AttackNode CurrentAttackNode { get; private set; }

    private AnimationHandler animationHandler;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
    }

    public void SetCombo(ComboSet comboSet)
    {
        weaponComboSet = comboSet;
    }

    public void IsLightAttackNode(bool isLightAttackNode)
    {
        CurrentAttackNode = isLightAttackNode? weaponComboSet.FirstLightAttack : weaponComboSet.FirstHeavyAttack;
    }
    public void ChargeAttack()
    {
        CurrentAttackNode = weaponComboSet.ChargeAttack;
    }
    public void AirAttack()
    {
        CurrentAttackNode = weaponComboSet.AirAttack;
    }
    public void ParryAttack()
    {
        CurrentAttackNode = weaponComboSet.ParryAttack;
    }

    public void NextAttackNode(bool isLightAttack)
    {
        AttackNode nextNode = isLightAttack ? CurrentAttackNode.nextLightAttackNode : CurrentAttackNode.nextHeavyAttackNode;

        if (nextNode != null)
        {
            CurrentAttackNode = nextNode;
        }
        else if (CurrentAttackNode.noNextAttack)
        {
            CurrentAttackNode = isLightAttack ? weaponComboSet.FirstLightAttack : weaponComboSet.FirstHeavyAttack;
        }
        else
        {
            return;
        }

    }

    public bool IsInComboWindow()
    {
        if (CurrentAttackNode == null || CurrentAttackNode.noNextAttack)
        {
            return false;
        }

        float t = animationHandler.NormalizedTime();
        return CurrentAttackNode.comboWindowStart <= t && t <= CurrentAttackNode.comboWindowEnd;
    }

    public void PlayAttack()
    {
        animationHandler.Play(CurrentAttackNode.attackName);
    }
    public void PlayNextAttack()
    {
        animationHandler.CrossFade(CurrentAttackNode.attackName, 0.1f);
    }

    public void ResetCombo()
    {
        CurrentAttackNode = null;
    }

    //this is called in an animation event
    public void SetSubAttackNode(int num)
    {
        if(CurrentAttackNode != null)
        {
            CurrentAttackNode = CurrentAttackNode.subAttackNodes[num];
        }
    }
}
