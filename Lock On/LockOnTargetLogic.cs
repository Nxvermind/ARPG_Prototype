using Unity.Cinemachine;
using UnityEngine;

public class LockOnTargetLogic : MonoBehaviour
{
    public static Transform CurrentLockOnTarget { get; private set; }

    [SerializeField] private TargetingSystem targetingSystem;

    public CinemachineCamera LockOnCamera { get; private set; }
    private Camera mainCamera;

    [Space]
    [Tooltip("Offset applied to adjust the origin point used when searching for the lock-on target. Helps align detection with the player's position or camera view.")]
    [SerializeField] private Vector3 offset;

    [Header("Ray Settings")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Mouse Aim Settings")]
    [SerializeField] private float aimSensitivity;
    [SerializeField] private float maxAngleOffset;
    public float currentAngleOffset;

    [Header("Find Initial Target")]
    [SerializeField] private float firstAngle;
    [SerializeField] private float secondAngle;

    private float timeToResetAngleOffset;

    private LockOnTargetBlackboard lockOnTargetBlackboard;

    private void Awake()
    {
        lockOnTargetBlackboard = GetComponent<LockOnTargetBlackboard>();
        LockOnCamera = GetComponent<CinemachineCamera>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        EventBus.OnEnemyDeathEvent += HandleEnemyDeath;
    }
    private void OnDisable()
    {
        EventBus.OnEnemyDeathEvent -= HandleEnemyDeath;
    }

    public void SetLockOnTarget(Transform target)
    {
        CurrentLockOnTarget = target;


        if (CurrentLockOnTarget && CurrentLockOnTarget.TryGetComponent(out EnemyLockOnIndicator indicator))
        {
            indicator.ActiveLockOnIndicator();
            LockOnCamera.LookAt = CurrentLockOnTarget;
        }
    }

    public void DeactivateLockOn()
    {
        if (CurrentLockOnTarget && CurrentLockOnTarget.TryGetComponent(out EnemyLockOnIndicator indicator))
        {
            indicator.DeactiveLockOnIndicator();
        }

        CurrentLockOnTarget = null;
        currentAngleOffset = 0;
        
        lockOnTargetBlackboard.Deactivate();
    }

    private void HandleEnemyDeath(Enemy deadEnemy)
    {
        if(CurrentLockOnTarget == null) return;

        if(CurrentLockOnTarget.parent == deadEnemy.transform)
        {
            Debug.Log("handle enemy death called");
            if (CurrentLockOnTarget && CurrentLockOnTarget.TryGetComponent(out EnemyLockOnIndicator indicator))
            {
                indicator.DeactiveLockOnIndicator();
            }

            Transform target = FindTargetAfterDeath();

            if (target == null)
            {
                Debug.Log("called deactivateLockOn");
                DeactivateLockOn();
            }
            else
            {
                Debug.Log($"target found {target.name}");
                SetLockOnTarget(target);
            }
        }


    }

    public void HandleMouseAim()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");

        if (mouseX == 0)
        {
            if(Time.time >= timeToResetAngleOffset + 1)
            {
                if(currentAngleOffset < -35)
                {
                    currentAngleOffset = -35;
                }
                else if(currentAngleOffset > 35)
                {
                    currentAngleOffset = 35;
                }
            }

            return;
        }

        if (mouseX != 0)
        {
            timeToResetAngleOffset = Time.time;
        }

        currentAngleOffset = Mathf.Clamp(currentAngleOffset + mouseX * aimSensitivity, -maxAngleOffset, maxAngleOffset);

        UpdateLockOnTarget();
    }

    private void UpdateLockOnTarget()
    {
        Vector3 origin = mainCamera.transform.TransformPoint(offset);
        Quaternion rotation = Quaternion.AngleAxis(currentAngleOffset, mainCamera.transform.up);
        Vector3 direction = rotation * mainCamera.transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, enemyLayer))
        {
            Transform hitTransform = hit.transform;

            if (hitTransform == CurrentLockOnTarget) return;

            if (CurrentLockOnTarget.TryGetComponent(out EnemyLockOnIndicator lastIndicator))
            {
                lastIndicator.DeactiveLockOnIndicator();
            }

            if (hitTransform.TryGetComponent(out EnemyLockOnIndicator newIndicator))
            {
                newIndicator.ActiveLockOnIndicator();
            }

            CurrentLockOnTarget = hitTransform;
            LockOnCamera.LookAt = CurrentLockOnTarget;
        }

        Debug.DrawRay(origin, direction * maxDistance, Color.yellow);
    }

    public Transform FindTarget()
    {
        Vector3 origin = mainCamera.transform.TransformPoint(offset);

        Vector3 forward = mainCamera.transform.forward;

        Transform target = targetingSystem.FindClosestEnemyInVisionAngleFromPosition(origin, forward, firstAngle);

        if (target == null)
        {
            target = targetingSystem.FindClosestEnemyInVisionAngleFromPosition(origin, forward, secondAngle);
        }

        return target;
    }

    public Transform FindTargetAfterDeath()
    {
        Vector3 origin = mainCamera.transform.TransformPoint(offset);
        return targetingSystem.FindClosestEnemyForLockOn(origin, CurrentLockOnTarget);
    }
}
