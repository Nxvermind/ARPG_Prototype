using UnityEngine;

public class EnemyCombatStatus : MonoBehaviour
{
    [Header("Attack Related")]
    public bool isActiveAttacker;

    public bool waitingForAttackPermission;

    public bool isAllowedToAttack;

    public bool oneTimeAttackPermission;

    public float timeSinceWaitingForAttackPermission;

    //if the melee enemy is too close to the player then this time will decrement till 0 then the enemy can attack one time
    public float timeToGrantAttackPermission;

    [Tooltip("if enemy doesnt attack in this range of time then will change the active attacker")]
    public float limitTimeWithoutAttacking;

    public float originalLimitTime;

    [Header("If its range Enemy")]
    //range enemy has other rules for the enemyAttackCoordinator

    public bool readyToAttack;
    public float timeToLetRangeEnemyAttack;

    private void Start()
    {
        originalLimitTime = limitTimeWithoutAttacking;
        isAllowedToAttack = true;
    }

}
