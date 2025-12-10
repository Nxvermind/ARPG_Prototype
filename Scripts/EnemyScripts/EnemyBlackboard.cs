using UnityEngine;

public class EnemyBlackboard : MonoBehaviour
{
    public bool gotHit;

    public bool rememberedGotHit;

    public bool gotHitByUltimateSkill;

    public bool gotHitByDashSkill;

    public float gotHitLastTime;

    public bool onlyTakeDamage;

    public bool isAttacking;

    public bool canUpdateStaggerValue;

    public float timeToStartStaggerRegeneration;
    [Space]

    [HideInInspector] public Vector3 lastKnownPlayerPosition;

    [HideInInspector] public Vector3 positionToStartSearching;

    [HideInInspector] public bool isDead;

    public bool IsTurningAround { get; set; }

    [Header("Execution")]

    public bool isBeingExecuted;

    public bool wasExecuted;

    public bool canExecuteThisEnemy;


    public void OnlyTakeDamageActive()
    {
        onlyTakeDamage = true;
    }

    public void OnlyTakeDamageInactive()
    {
        onlyTakeDamage = false;
    }
}
