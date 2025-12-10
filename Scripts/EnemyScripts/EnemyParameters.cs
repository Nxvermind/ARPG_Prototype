using UnityEngine;

public class EnemyParameters : MonoBehaviour 
{
    [Header("Health Info")]
    public float currentHp;
    public float maxHp;
    [Space]
    public float maxStaggerValue;
    public float currentStaggerValue;
    public float regenStaggerValue;
    [Space]
    public float patrolSpeed;
    public float aggresiveWalkSpeed;
    [Tooltip("time in seconds the enemy walks instead of chasing")]
    public float aggresiveWalkTime;
    public float chaseSpeed;
    [Header("Attack Info")]
    [Tooltip("the distance needed for this enemy to attack the player")]
    public float distanceToAttack;
}
