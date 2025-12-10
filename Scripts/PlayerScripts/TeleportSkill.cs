using Unity.Cinemachine;
using UnityEngine;

public class TeleportSkill : MonoBehaviour
{
    private EnemyDetector enemyDetector;

    public float angleToFindEnemy;   

    public Transform currentTarget;

    public Transform player;

    public CinemachineOrbitalFollow cm;

    private InputHandler inputHandler;

    private SkillUIManager skillManager;

    private void Start()
    {
        enemyDetector = transform.parent.GetComponentInChildren<EnemyDetector>();
        inputHandler = GetComponentInParent<InputHandler>();
        skillManager = GetComponentInParent<SkillUIManager>();
    }

    void Update()
    {
        if (SwitchCameras.IsLockOnTargetCameraActive)
        {
            currentTarget = enemyDetector.CurrentTarget;
        }

        if(enemyDetector.NumOfEnemiesDetected != 0)
        {
            currentTarget = enemyDetector.FindClosestEnemyInVisionAngle(transform, angleToFindEnemy);
        }

        if (inputHandler.TeleportSkill && skillManager.data["TeleportSkill"].isSkillReady && currentTarget != null)
        {
            ActivateTeleportSkill();
            skillManager.StartSkillCooldown("TeleportSkill");
        }
        else if (inputHandler.TeleportSkill && !skillManager.data["TeleportSkill"].isSkillReady && currentTarget != null)
        {
            SkillOnCooldownSFX.instance.PlaySFX();
        }
    }

    private void ActivateTeleportSkill()
    {
        if (currentTarget == null) return;

        if (currentTarget.TryGetComponent<EnemyVision>(out var targetBlackboard))
        {
            targetBlackboard.playerInSight = false;
            targetBlackboard.playerDisappeared = true;
        }

        Vector3 backPosition = currentTarget.position - currentTarget.forward * 1.2f - currentTarget.right * 0.3f;
        backPosition.y = player.position.y;

        float mappedAngle = Mathf.DeltaAngle(0, currentTarget.eulerAngles.y);

        cm.HorizontalAxis.Value = mappedAngle;
        cm.VerticalAxis.Value = 17.5f;

        player.SetPositionAndRotation(backPosition, currentTarget.rotation);
    }
}
