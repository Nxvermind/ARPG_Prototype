using System;
using UnityEngine;

public static class PlayerEvents
{
    public static event Action OnPlayerBlockEvent;

    public static event Action OnPlayerHitEvent;

    public static event Action OnPlayerDeath;

    public static event Action OnSuccessfulParryEvent;

    public static event Action OnPlayerAttackButtonPressed;

    public static event Action OnUltimateSkillCalled;

    public static void PlayerBlockEvent()
    {
        OnPlayerBlockEvent?.Invoke();
    }

    public static void SuccessfulParryEvent()
    {
        OnSuccessfulParryEvent?.Invoke();
    }

    public static void PlayerGotHitEvent()
    {
        OnPlayerHitEvent?.Invoke();
    }

    public static void PlayerDeathEvent()
    {
        OnPlayerDeath?.Invoke();
    }

    public static void UltimateSkillCalled()
    {
        OnUltimateSkillCalled?.Invoke();
    }

    public static void AttackButtonPressed()
    {
        OnPlayerAttackButtonPressed?.Invoke();
    }
}
