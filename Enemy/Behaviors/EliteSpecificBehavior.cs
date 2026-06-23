using UnityEngine;

public class EliteSpecificBehavior : EnemySpecificBehavior
{
    private EliteEnemy enemy;


    public override void InitializeBehavior(Enemy enemy)
    {
        this.enemy = enemy as EliteEnemy;
    }

    public override void UpdateSpecificBehavior()
    {
        if (enemy.StateMachine.CurrentState == enemy.eliteEnemyStateFactory.SecondPhaseState) return;

        if(enemy.EnemyParameters.currentStaggerValue >= enemy.EnemyParameters.maxStaggerValue && enemy.EnemyGroundDetection.isGrounded && !enemy.EnemyBlackboard.isIncapacitated)
        {
            enemy.StateMachine.ChangeState(enemy.eliteEnemyStateFactory.StunState);
            return;
        }

        if (enemy.SkillSmash.isSkillUnlocked && enemy.SkillSmash.skillSmashReady && 
            Time.time >= enemy.EnemyBlackboard.gotHitLastTime + 2 && Time.time >= enemy.EnemyBlackboard.lastTimeSinceAttack + 2)
        {
            enemy.StateMachine.ChangeState(enemy.eliteEnemyStateFactory.SkillSmashState);
            return;
        }

        if(enemy.EnemyVision.isPlayerInSight)
        {
            if(enemy.AssaultSkill.SkillUnlocked && enemy.AssaultSkill.IsSkillAvailable() && 
               enemy.StateMachine.CurrentState != enemy.eliteEnemyStateFactory.GetReadyToAssaultSkillState &&
               enemy.AssaultSkill.InRangeToActivateSkill(enemy.target) && enemy.EnemyGroundDetection.isGrounded)
            {
                enemy.StateMachine.ChangeState(enemy.eliteEnemyStateFactory.AssaultSkillState);
                return;
            }

            if (enemy.AssaultSkill.SkillUnlocked && enemy.AssaultSkill.IsSkillAvailable() && !enemy.EnemyBlackboard.isAttacking &&
                enemy.StateMachine.CurrentState != enemy.eliteEnemyStateFactory.StunState && enemy.IsPlayerInRange(5) && 
                enemy.EnemyGroundDetection.isGrounded && Time.time >= enemy.EnemyBlackboard.lastTimeSinceAttack + 1.3f)
            {
                enemy.StateMachine.ChangeState(enemy.eliteEnemyStateFactory.GetReadyToAssaultSkillState);
                return;
            }

        }
    }
}
