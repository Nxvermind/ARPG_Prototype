using UnityEngine;

/// <summary>
/// Factory responsible for creating and providing all Player FSM states.
/// Called once during player initialization to instantiate every state with
/// Acts as a centralized registry so transitions can be made cleanly without creating new state instances during gameplay.
/// </summary>

public class PlayerStateFactory 
{
    #region States

    [Header("Intro")]
    public PlayerIntroState IntroState { get; private set; }

    [Header("Locomotion")]
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerDodgeState DodgeState { get; private set; }
    public PlayerGotHitState GotHitState { get; private set; }

    [Header("Falling")]
    public PlayerFallingState FallingState { get; private set; }

    [Header("Ground_Attack")]
    public PlayerGroundAttackState GroundAttackState { get; private set; }
    public PlayerGroundChargeAttackState GroundChargeAttackState { get; private set; }
    public PlayerParryState ParryState { get; private set; }
    public PlayerPostureBrokenState PostureBrokenState { get; private set; }

    [Header("Air_Attack")]
    public PlayerAirAttackState AirAttackState { get; private set; }
    public PlayerAirChargeAttackState AirChargeAttackState { get; private set; }

    [Header("Skills")]
    public PlayerSkillDashState SkillDash { get; private set; }
    public PlayerUltimateSkillState UltimateSkillState { get; private set; }

    [Header("Death")]
    public PlayerDeathState DeathState { get; private set; }

    [Header("Execution")]
    public PlayerExecutionState ExecutionState { get; private set; }

    #endregion

    public void InitializeState(Player player, StateMachine<Player> stateMachine)
    {
        IdleState = new PlayerIdleState(player, stateMachine);
        WalkState = new PlayerWalkState(player, stateMachine);
        RunState = new PlayerRunState(player, stateMachine);
        DodgeState = new PlayerDodgeState(player, stateMachine);
        FallingState = new PlayerFallingState(player, stateMachine);
        GroundAttackState = new PlayerGroundAttackState(player, stateMachine);
        GroundChargeAttackState = new PlayerGroundChargeAttackState(player, stateMachine);
        AirAttackState = new PlayerAirAttackState(player, stateMachine);
        AirChargeAttackState = new PlayerAirChargeAttackState(player, stateMachine);
        SkillDash = new PlayerSkillDashState(player, stateMachine);
        DeathState = new PlayerDeathState(player, stateMachine);
        ParryState = new PlayerParryState(player, stateMachine);
        GotHitState = new PlayerGotHitState(player, stateMachine);
        ExecutionState = new PlayerExecutionState(player, stateMachine);
        PostureBrokenState = new PlayerPostureBrokenState(player, stateMachine);
        UltimateSkillState = new PlayerUltimateSkillState(player, stateMachine);
        IntroState = new PlayerIntroState(player, stateMachine);
    }
}
