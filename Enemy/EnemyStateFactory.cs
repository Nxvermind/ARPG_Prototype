using UnityEngine;

public class EnemyStateFactory
{
    public EnemyIdleState IdleState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyDeathState DeathState { get; private set; }
    public EnemyAlertState AlertState { get; private set; }
    public EnemyGroundGotHitState GroundGotHitState { get; private set; }
    public EnemyAirGotHitState AirGotHitState { get; private set; }
    public EnemyStaggerState StaggerState { get; private set; }
    public EnemyAggresiveWalkState AggresiveWalkState { get; private set; }
    public EnemyWaitingForAttackState WaitingForAttackState { get; private set; }
    public EnemyPreparingToAttackState PreparingtoAttack { get; private set; }
    public EnemyParriedState ParriedState { get; private set; }
    public EnemyGetUpState GetUpState { get; private set; }
    public EnemyTurnAroundState TurnAroundState { get; private set; }
    public EnemyLookingForPlayerState LookingForPlayerState { get; private set; }
    public EnemyFallingState FallingState { get; private set; }

    public virtual void InitializeState(Enemy enemy, EnemyStateFactory enemyStateFactory ,StateMachine<Enemy> stateMachine)
    {
        IdleState = new EnemyIdleState(enemy, this, stateMachine);
        ChaseState = new EnemyChaseState(enemy, this, stateMachine);
        AttackState = new EnemyAttackState(enemy, this, stateMachine);
        DeathState = new EnemyDeathState(enemy, this, stateMachine);
        GroundGotHitState = new EnemyGroundGotHitState(enemy, this, stateMachine);
        AirGotHitState = new EnemyAirGotHitState(enemy, this, stateMachine);
        StaggerState = new EnemyStaggerState(enemy, this, stateMachine);      
        AggresiveWalkState = new EnemyAggresiveWalkState(enemy, this, stateMachine);
        WaitingForAttackState = new EnemyWaitingForAttackState(enemy, this, stateMachine);
        ParriedState = new EnemyParriedState(enemy, this, stateMachine);
        PreparingtoAttack = new EnemyPreparingToAttackState(enemy, this, stateMachine);
        GetUpState = new EnemyGetUpState(enemy, this, stateMachine);
        AlertState = new EnemyAlertState(enemy, this, stateMachine);
        TurnAroundState = new EnemyTurnAroundState(enemy, this, stateMachine);
        LookingForPlayerState = new EnemyLookingForPlayerState(enemy, this, stateMachine);
        FallingState = new EnemyFallingState(enemy, this, stateMachine);
    }
}
