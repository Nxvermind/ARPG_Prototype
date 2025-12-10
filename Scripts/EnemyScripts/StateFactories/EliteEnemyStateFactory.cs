using UnityEngine;

public class EliteEnemyStateFactory : EnemyStateFactory
{
    public EliteEnemySkillSmashState SkillSmashState { get; private set; }
    public EliteEnemyStunState StunState { get; private set; }

    public EliteEnemyEvadeState EvadeState { get; private set; }

    public EliteEnemySecondPhaseState SecondPhaseState { get; private set; }

    public override void InitializeState(Enemy enemy, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine)
    {
        base.InitializeState(enemy, enemyStateFactory, stateMachine);

        var eliteEnemy = enemy as EliteEnemy;

        SkillSmashState = new EliteEnemySkillSmashState(eliteEnemy, this, stateMachine);
        StunState = new EliteEnemyStunState(eliteEnemy, this, stateMachine);
        EvadeState = new EliteEnemyEvadeState(eliteEnemy, this, stateMachine);
        SecondPhaseState = new EliteEnemySecondPhaseState(eliteEnemy, this, stateMachine);
    }
}
