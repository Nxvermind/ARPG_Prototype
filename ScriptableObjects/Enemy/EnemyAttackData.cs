using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/AttackData")]
public class EnemyAttackData : ScriptableObject
{
    public string attackAnimationName;
    public int damage;
    public int postureDamage;

    public bool isParryable;
}
