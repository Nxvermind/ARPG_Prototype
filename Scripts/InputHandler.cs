using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public bool LightAttackButtonPressed { get; private set; }

    public bool LightAttackButtonHeld { get; private set; }

    public bool LightAttackButtonReleased { get; private set; }

    public bool HeavyAttackButtonPressed { get; private set; }

    public bool HeavyAttackButtonHeld { get; private set; }

    public bool HeavyAttackButtonReleased { get; private set; }

    public bool ParryInputPressed { get; private set; }

    public bool ParryInputHeld { get; private set; }

    public bool ParryInputUp { get; private set; } 

    public bool SpaceKey { get; private set; }

    public bool LeftShift { get; private set; }

    public bool SkillDashButton { get; private set; }

    public bool LightSwordButton { get; private set; }

    public bool GreatSwordButton { get; private set; }

    public bool ChargeAttackButton { get; private set; }

    public bool LockOnTargetButton { get; private set; }

    public bool ExecutionButton { get; private set; }

    public bool TeleportSkill { get; private set; }

    public bool UltimateSkillButton { get; private set; }

    private void Update()
    {
        LightAttackButtonPressed = Input.GetMouseButtonDown(0);
        LightAttackButtonHeld = Input.GetMouseButton(0);
        LightAttackButtonReleased = Input.GetMouseButtonUp(0);

        HeavyAttackButtonHeld = Input.GetMouseButton(1);
        HeavyAttackButtonPressed = Input.GetMouseButtonDown(1);
        HeavyAttackButtonHeld = Input.GetMouseButtonUp(1);

        LockOnTargetButton = Input.GetMouseButtonDown(2);

        ParryInputPressed = Input.GetKeyDown(KeyCode.Q);
        ParryInputHeld = Input.GetKey(KeyCode.Q);
        ParryInputUp = Input.GetKeyUp(KeyCode.Q);

        SpaceKey = Input.GetKeyDown(KeyCode.Space);

        ExecutionButton = Input.GetKeyDown(KeyCode.E);

        LeftShift = Input.GetKey(KeyCode.LeftShift);

        LightSwordButton = Input.GetKeyDown(KeyCode.F1);

        GreatSwordButton = Input.GetKeyDown(KeyCode.F2);

        SkillDashButton = Input.GetKeyDown(KeyCode.Alpha1);

        ChargeAttackButton = Input.GetKeyDown(KeyCode.Alpha2);

        TeleportSkill = Input.GetKeyDown(KeyCode.Alpha3);

        UltimateSkillButton = Input.GetKeyDown(KeyCode.R);
    }
}
