using Unity.Cinemachine;
using UnityEngine;

public class LockOnTargetCamera : MonoBehaviour
{
    public static Transform CurrentLockOnTarget { get; private set; }

    [SerializeField] private EnemyDetector enemyDetector;

    private CinemachineCamera lockOnCamera;
    private Camera mainCamera;

    public bool noEnemiesDetected;
    [Space]
    [Tooltip("Offset applied to adjust the origin point used when searching for the lock-on target. Helps align detection with the player's position or camera view.")]
    [SerializeField] private Vector3 offset;

    [Header("Ray Settings")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Mouse Aim Settings")]
    [SerializeField] private float aimSensitivity;
    [SerializeField] private float maxAngleOffset;
    private float currentAngleOffset;

    [Header("Find Initial Target")]
    [SerializeField] private float firstAngle;
    [SerializeField] private float secondAngle;

    private void Awake()
    {
        lockOnCamera = GetComponent<CinemachineCamera>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        EventBus.OnEnemyDeathEvent += HandleEnemyDeath;
        SwitchCameras.OnLockOntargetButtonPressed += SetLockOnTarget;
    }
    private void OnDestroy()
    {
        EventBus.OnEnemyDeathEvent -= HandleEnemyDeath;
        SwitchCameras.OnLockOntargetButtonPressed -= SetLockOnTarget;
    }

    private void Update()
    {
        if (!SwitchCameras.IsLockOnTargetCameraActive)
        {
            DeactivateLockOn();
            return;
        }

        HandleMouseAim();
    }

    private void SetLockOnTarget()
    {
        Transform target = FindTarget();

        if(target == null)
        {
            DeactivateLockOn();
            return;
        }

        CurrentLockOnTarget = target;

        lockOnCamera.LookAt = CurrentLockOnTarget;

        if (CurrentLockOnTarget && CurrentLockOnTarget.TryGetComponent(out EnemyLockOnIndicator indicator))
        {
            indicator.lockOnIndicatorImage.enabled = true;
        }
        //Debug.Log($"current lock on target is {target} ");
    }

    private void DeactivateLockOn()
    {
        if (CurrentLockOnTarget && CurrentLockOnTarget.TryGetComponent(out EnemyLockOnIndicator indicator))
        {
            indicator.lockOnIndicatorImage.enabled = false;
        }

        CurrentLockOnTarget = null;
        currentAngleOffset = 0;
        noEnemiesDetected = false;
    }

    private void HandleEnemyDeath(Enemy deadEnemy)
    {
        if(CurrentLockOnTarget == deadEnemy.transform)
        {
            if (CurrentLockOnTarget && CurrentLockOnTarget.TryGetComponent(out EnemyLockOnIndicator indicator))
            {
                indicator.lockOnIndicatorImage.enabled = false;
            }

            Transform target = FindTarget();

            if(target == null)
            {
                noEnemiesDetected = true;
            }
            else
            {
                SetLockOnTarget();
            }     
        }
    }

    private void HandleMouseAim()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");

        if (mouseX == 0)
        {
            return;
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

            if (hitTransform == CurrentLockOnTarget)
            {
                return;

            }

            if (CurrentLockOnTarget && CurrentLockOnTarget.TryGetComponent(out EnemyLockOnIndicator lastIndicator))
            {
                lastIndicator.lockOnIndicatorImage.enabled = false;
            }

            if (hitTransform.TryGetComponent(out EnemyLockOnIndicator newIndicator))
            {
                newIndicator.lockOnIndicatorImage.enabled = true;
            }

            CurrentLockOnTarget = hitTransform;
            lockOnCamera.LookAt = CurrentLockOnTarget;
        }

        Debug.DrawRay(origin, direction * maxDistance, Color.yellow);
    }

    public Transform FindTarget()
    {
        Vector3 origin = mainCamera.transform.TransformPoint(offset);

        Vector3 forward = mainCamera.transform.forward;

        Transform target = enemyDetector.FindClosestEnemyInVisionAngleFromPosition(origin, forward, firstAngle);

        if (target == null)
        {
            target = enemyDetector.FindClosestEnemyInVisionAngleFromPosition(origin, forward, secondAngle);
        }

        return target;
    }

    //private void OnDrawGizmos()
    //{
    //    if(mainCamera == null)
    //    {
    //        mainCamera = Camera.main;
    //    }

    //    Gizmos.color = Color.yellow;

    //    // Calcula la posición mundial del punto de origen del offset
    //    Vector3 offsetWorldPos = mainCamera.transform.TransformPoint(offset);

    //    // Direcciones del cono de visión
    //    Vector3 leftDir = Quaternion.Euler(0, -firstAngle, 0) * mainCamera.transform.forward;
    //    Vector3 rightDir = Quaternion.Euler(0, firstAngle, 0) * mainCamera.transform.forward;

    //    Gizmos.DrawLine(offsetWorldPos, offsetWorldPos + leftDir * 30);
    //    Gizmos.DrawLine(offsetWorldPos, offsetWorldPos + rightDir * 30);

    //    Vector3 secondLeftDir = Quaternion.Euler(0, -secondAngle, 0) * mainCamera.transform.forward;
    //    Vector3 secondRightDir = Quaternion.Euler(0, secondAngle, 0) * mainCamera.transform.forward;

    //    Gizmos.DrawLine(offsetWorldPos, offsetWorldPos + secondLeftDir * 30);
    //    Gizmos.DrawLine(offsetWorldPos, offsetWorldPos + secondRightDir * 30);
    //}
}
