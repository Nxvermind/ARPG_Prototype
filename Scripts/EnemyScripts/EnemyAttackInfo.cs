using UnityEngine;

public readonly struct EnemyAttackInfo 
{
    public readonly Enemy enemy;

    public readonly EnemyAttackData AttackData;

    public EnemyAttackInfo(Enemy enemy, EnemyAttackData attackData)
    {
        this.enemy = enemy;
        AttackData = attackData;
    }
}
