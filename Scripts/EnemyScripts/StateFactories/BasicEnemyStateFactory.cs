using UnityEngine;

public class BasicEnemyStateFactory : EnemyStateFactory
{
    public EnemyExecutedState ExecutedState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyBackToPatrolPointState BackToPatrolPointState { get; private set; }
    public EnemyJumpBackwardsState JumpBackwardsState { get; private set; }

    public BasicEnemyPrayingState PrayingState { get; private set; }

    public override void InitializeState(Enemy enemy, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine)
    {
        base.InitializeState(enemy, enemyStateFactory, stateMachine);

        var basicEnemy = enemy as BasicEnemy;

        PatrolState = new EnemyPatrolState(basicEnemy, this, stateMachine);
        BackToPatrolPointState = new EnemyBackToPatrolPointState(basicEnemy, this, stateMachine);
        JumpBackwardsState = new EnemyJumpBackwardsState(basicEnemy, this, stateMachine);
        ExecutedState = new EnemyExecutedState(basicEnemy, this, stateMachine);

        PrayingState = new BasicEnemyPrayingState(basicEnemy, this, stateMachine);
    }
}
