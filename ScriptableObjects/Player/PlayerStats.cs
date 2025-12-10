using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public float maxHP;
    public float currentHp;
    [Space]
    public float maxStamina;
    public float currentStamina;
    [Space]
    public float maxPostureValue;
    public float currentPostureValue; //if it is 0 then player gets vulnerable for a short time
    public float regenPostureValue;
    public bool postureBroken;
    [Space]
    public float walkSpeed;
    public float runSpeed;
    public float dodgeForce;
    public float dodgeCooldown;
    [Space]
    public float regenBerserkValue;
    public float currentBerserkValue;
    public float maxBerserkValue;
    public float LastDodgeTime { get; set; }
}
