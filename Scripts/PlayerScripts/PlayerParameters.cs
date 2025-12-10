using UnityEngine;

public class PlayerParameters : MonoBehaviour 
{
    public float maxHP;
    public float currentHp;
    [Space]
    public float maxStamina;
    public float currentStamina;
    public float regenStaminaValue;
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
    public float regenUltimateSkillValue;
    public float currentUltimateSkillValue;
    public float maxUltimateSkillValue;
    public float LastDodgeTime { get; set; }
}
