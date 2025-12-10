using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Combo/AttackNode")]
public class AttackNode : ScriptableObject
{
    [Tooltip("This is the name of the animation in the animator window. It has to be exactly the name of the animation")]
    public string attackName; //si hay mas de 1 ataque en el mismo clip de animacion entonces ambos ataques deben tener el mismo nombre
    public string slashAttackName;
    public float staggerDamage;
    public float damage;

    [Header("Attack Direction")]
    public float strongAttackForce;
    public bool upStrongAttack;
    public bool botStrongAttack;

    [Header("ComboSystem")]
    [Tooltip("De 0 a 1 (para el porcentaje del animator.normalizedTime), el numero elegido es donde se abre la ventana para que el player pueda seguir atacando")]
    public float comboWindowStart;
    [Tooltip("Lo mismo que el comboWindowStart pero esta vez para que se cierre y que regrese al primer ataque")]
    public float comboWindowEnd;
    [Space]
    public float inputBuffer;

    [Header("Expecting next attack node")]
    public AttackNode nextLightAttackNode;
    public AttackNode nextHeavyAttackNode;

    [Space]
    [Tooltip("Is this the final attack of the current attack secuence?")]
    public bool noNextAttack;
    [Tooltip("normalized time of the animation to be able to restart the first attack")]
    public float timeToLetRestartCombo;

    [Header("SFX")]
    public AudioClip attackSoundFX;

    [Space]
    [Header("SubAttacks")]
    public bool hasSubAttacks;
    [ShowIf("hasSubAttacks")]
    public List<AttackNode> subAttackNodes;
}
