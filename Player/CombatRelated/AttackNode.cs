using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum HitReactionType
{
    None,
    PushedBack
}

public enum AttackDirection
{
    None,
    Left,
    Right,
    Up,
    Down
}

[CreateAssetMenu(menuName = "Combo/AttackNode")]
public class AttackNode : ScriptableObject
{
    [Tooltip("This is the name of the animation in the animator window. It has to be exactly the name of the state in the animator")]
    public string attackName; //If there is more than one attack in the same animation clip, both attacks must have the same name
    public float damage;
    public float staggerDamage;
    public float hitstopDuration;
    [Space]
    public bool isStrongAttack;

    [Header("Target Attraction")]
    public float targetAttractionDelay;

    [Header("CameraShake"), Tooltip("0 mutes the camera shake")]
    public float intensityMultiplier = 1;

    [Header("Hit Reaction")]
    public HitReactionType hitReactionType;

    [Header("Attack Direction")]
    public AttackDirection attackDirection;
    [Space]
    public float liftImpulse;

    [Header("ComboSystem")]
    [Tooltip("From 0 to 1 (representing the animator.normalizedTime percentage), " +
        "the selected value determines when the combo window opens, allowing the player to continue attacking"), Min(0), MaxValue(1)]
    public float comboWindowStart;
    [Tooltip("Same as comboWindowStart, but this time it determines when the combo window closes and the combo returns to the first attack"), Min(0), MaxValue(1)]
    public float comboWindowEnd;
    [Space]
    public float inputBuffer;

    [Header("Expecting next attack node")]
    public bool expectsNextAttack;
    public AttackNode nextLightAttackNode;
    public AttackNode nextHeavyAttackNode;

    [Space]
    [Tooltip("Is this the final attack of the current attack sequence?")]
    public bool noNextAttack;
    [ShowIf("noNextAttack"), Tooltip("normalized time of the animation to be able to restart the first attack")]
    public float timeToLetRestartCombo;
    [Space]
    [Tooltip("Percentage of animator.normalizedTime (from 0 to 1) at which the character transitions back to the Idle state after attacking"), Min(0), MaxValue(1)]
    public float exitToIdleTime;

    [Header("SFX")]
    public AudioClip attackSoundFX;

    [Space]
    [Header("SubAttacks")]
    public bool hasSubAttacks;
    [ShowIf("hasSubAttacks")]
    public List<AttackNode> subAttackNodes;
}
