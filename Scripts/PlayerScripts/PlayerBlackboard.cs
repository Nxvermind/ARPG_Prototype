using UnityEngine;

public class PlayerBlackboard : MonoBehaviour
{
    public float LastTimeToStartRegenStamina;
    public float timeToStartRegenStamina;
    public float lastTimeAttacked;

    public float invulneravilityTime;
    public bool isInvulnerable;

    [Header("Bools")]

    public bool canMove;
    public bool canRun;
    public bool canAttack = true;
    public bool canDodge;
    public bool isAttacking;
    public bool firstDodge = true;
    public bool isRunning;
    public bool canParry = true;
    public bool canChargeAttack = true;
    public bool inChargeAttack;

    public bool isPlayerDead;
    public bool onlyTakeDamage;

    public bool firstAttack;

    public bool ultimateSkillAvailable;


    [Header("GroundCheck")]
    public float checkGroundDistance;
    public Vector3 checkGroundOffset;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;

    [Header("ChargeAttack")]
    public float chargeStartTime;
    public float chargeThreshold;

}
