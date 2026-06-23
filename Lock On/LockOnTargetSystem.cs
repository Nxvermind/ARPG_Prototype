using System;
using UnityEngine;

public class LockOnTargetSystem : MonoBehaviour
{
    public LockOnTargetLogic LockOnTargetLogic { get; private set; }

    public LockOnTargetBlackboard BlackBoard { get; private set; }

    [SerializeField] private PlayerCombatContext combatContext;

    public static event Action TriggerExitLockOnEvent;

    private void Awake()
    {
        LockOnTargetLogic = GetComponent<LockOnTargetLogic>();
        BlackBoard = GetComponent<LockOnTargetBlackboard>();
    }

    private void OnEnable()
    {
        SkillsEvents.OnUltimateSkillStarted += BlackBoard.Deactivate;
        SkillsEvents.OnUltimateSkillStarted += LockOnTargetLogic.DeactivateLockOn;
        TriggerExitLockOnEvent += ExitLockOn;
    }

    private void OnDisable()
    {
        SkillsEvents.OnUltimateSkillStarted -= BlackBoard.Deactivate;
        SkillsEvents.OnUltimateSkillStarted -= LockOnTargetLogic.DeactivateLockOn;
        TriggerExitLockOnEvent -= ExitLockOn;
    }

    public void ToggleLockOnTarget()
    {
        if (BlackBoard.IsLockOnTargetActive)
        {
            BlackBoard.Deactivate();
            LockOnTargetLogic.DeactivateLockOn();
            return;
        }

        if (!combatContext.ThereAreEnemies) return;

        var target = LockOnTargetLogic.FindTarget();

        if (target == null) return; 

        LockOnTargetLogic.SetLockOnTarget(target);

        BlackBoard.Activate();
    }

    public static void ForceExitLockOn()
    {
        TriggerExitLockOnEvent?.Invoke();
    }

    public void ExitLockOn()
    {
        if (!BlackBoard.IsLockOnTargetActive) return;

        LockOnTargetLogic.LockOnCamera.LookAt = null;
        LockOnTargetLogic.DeactivateLockOn();
        BlackBoard.Deactivate();
    }
}
