using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SwitchCameras : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;

    [Header("IntroCamera")]
    [SerializeField] private CinemachineCamera introCamera;

    [Header("NormalCamera")]
    [SerializeField] private CinemachineCamera normalCamera;
    private CinemachineInputAxisController normalAxisController;
    private CinemachineOrbitalFollow normalOrbitalFollow;
    private Coroutine backToNormalCameraCoroutine;

    [Header("CombatCamera")]
    [SerializeField] private CinemachineCamera combatCamera;
    private CinemachineOrbitalFollow combatOrbitalFollow;

    [Header("ExecutionCamera")]
    [SerializeField] private CinemachineCamera executionCamera;
    [SerializeField] private ExecutionCamera executionCameraScript;
    [SerializeField] private ExecutionManager executionManager;

    [Header("LockOnCamera")]
    [SerializeField] private LockOnTargetCamera lockOnTargetScript;
    public CinemachineCamera lockOnTargetCamera;

    [Header("UltimateSkillCamera")]
    [SerializeField] private UltiCamera ultiCameraScript;

    [Space]

    public CinemachineCamera[] cameras;
    public CinemachineCamera CurrentActiveCinemachineCamera { get; private set; }

    float currentHorizontalValue;

    private EnemyDetector enemyDetector;

    #region bools

    public static bool IsLockOnTargetCameraActive { get; private set; }
    public static bool IsExecutionCameraActive { get; private set; }

    public bool thereAreEnemies;

    private bool backToNormalCameraCorActive;

    public static bool ForceExitLockOnTargetCamera;

    private bool b;

    public bool introFinished;

    #endregion

    #region Events

    public static event Action OnLockOntargetButtonPressed;

    #endregion

    private void Start()
    {
        normalOrbitalFollow = normalCamera.GetComponent<CinemachineOrbitalFollow>();
        normalAxisController = normalCamera.GetComponent<CinemachineInputAxisController>();
        combatOrbitalFollow = combatCamera.GetComponent<CinemachineOrbitalFollow>();
        enemyDetector = GetComponentInChildren<EnemyDetector>();
    }

    private void OnEnable()
    {
        EventBus.OnExecutionStarted += ExecutionStarted;
        IntroScript.OnIntroStarted += IntroStarted;
        IntroScript.OnIntroFinished += IntroFinished;
    }

    private void OnDisable()
    {
        EventBus.OnExecutionStarted -= ExecutionStarted;
        IntroScript.OnIntroStarted -= IntroStarted;
        IntroScript.OnIntroFinished -= IntroFinished;
    }

    void Update()
    {
        if (ultiCameraScript.skillCasted || !introFinished) return;

        currentHorizontalValue = combatOrbitalFollow.HorizontalAxis.Value;

        if (inputHandler.LockOnTargetButton && enemyDetector.enemiesDetected.Count > 0 && !IsLockOnTargetCameraActive)
        {
            TrySwitchLockOnTarget();
        }

        if (IsLockOnTargetCameraActive && (lockOnTargetScript.noEnemiesDetected || enemyDetector.enemiesDetected.Count == 0))
        {
            DeactivateLockOnTarget();
        }

        if (enemyDetector.NumOfEnemiesDetected > 0)
        {
            combatCamera.Priority = 60;
            normalCamera.Priority = 1;
            backToNormalCameraCorActive = false;

            if(backToNormalCameraCoroutine != null)
            {
                StopCoroutine(backToNormalCameraCoroutine);
            }

            if (!thereAreEnemies)
            {
                thereAreEnemies = true;
                UpdateCurrentActiveCamera();
            }
        }
        else
        {
            if (!backToNormalCameraCorActive)
            {
                backToNormalCameraCorActive = true;
                backToNormalCameraCoroutine = StartCoroutine(BackToNormalCameraInSeconds());
            }

            thereAreEnemies = false;
        }

        if (ForceExitLockOnTargetCamera)
        {
            //IsLockOnTargetCameraActive = false;
            ForceExitLockOnTargetCamera = false;
            BackToCombatCamera();
        }

        if (Input.GetKeyDown(KeyCode.B) && IsLockOnTargetCameraActive)
        {
            //IsLockOnTargetCameraActive = false;
            BackToCombatCamera();
        }

        if (executionCameraScript.ExecutionFinished && b)
        {

            StartCoroutine(DelayExecutionCameraInactive());

            executionCamera.LookAt = null;  

            b = false;

            executionCamera.Priority = 1;
            combatCamera.Priority = 70;

            combatOrbitalFollow.HorizontalAxis.Value = currentHorizontalValue - 180;
            combatOrbitalFollow.VerticalAxis.Value = 17.5f;
        }
    }

    private void TrySwitchLockOnTarget()
    {
        OnLockOntargetButtonPressed?.Invoke();

        if (!IsLockOnTargetCameraActive)
        {
            if (lockOnTargetScript.FindTarget() == null)
            {
                Debug.Log(" No visible enemies — lock-on not activated.");
                return;
            }
            else if (lockOnTargetScript.FindTarget() != null)
            {
                IsLockOnTargetCameraActive = true;

                lockOnTargetCamera.Priority = 90;
                normalCamera.Priority = 1;

                UpdateCurrentActiveCamera();
            }
        }
    }

    private void UpdateCurrentActiveCamera()
    {
        float highestValue = float.MinValue;

        foreach (var cam in cameras)
        {
            if (cam.Priority.Value > highestValue)
            {
                highestValue = cam.Priority.Value;
                CurrentActiveCinemachineCamera = cam;
            }
        }
    }

    public CinemachineCamera NormalCamera()
    {
        return normalCamera;
    }

    public CinemachineCamera CombatCamera()
    {
        return combatCamera;
    }

    private void ExecutionStarted()
    {
        executionCamera.LookAt = executionManager.currentExecutableEnemy.transform;

        IsExecutionCameraActive = true;
        executionCamera.Priority = 80;
        b = true;
    }

    IEnumerator BackToNormalCameraInSeconds()
    {

        if (CurrentActiveCinemachineCamera == normalCamera) yield break;

        yield return new WaitForSecondsRealtime(5);

        normalOrbitalFollow.HorizontalAxis.Value = combatOrbitalFollow.HorizontalAxis.Value;
        normalOrbitalFollow.VerticalAxis.Value = combatOrbitalFollow.VerticalAxis.Value;

        normalCamera.Priority = 70;
        combatCamera.Priority = 1;


        UpdateCurrentActiveCamera();

    }

    private void BackToCombatCamera()
    {
        if (enemyDetector.NumOfEnemiesDetected == 0)
        {
            normalCamera.Priority = 20;
            combatCamera.Priority = 1;
            lockOnTargetCamera.Priority = 1;
        }
        else
        {
            combatCamera.Priority = 20;
            normalCamera.Priority = 1;
            lockOnTargetCamera.Priority = 1;
        }

        IsLockOnTargetCameraActive = false;
        UpdateCurrentActiveCamera();

    }

    IEnumerator DelayExecutionCameraInactive()
    {
        yield return new WaitForSecondsRealtime(.8f);
        IsExecutionCameraActive = false;
    }

    private void DeactivateLockOnTarget()
    {
        IsLockOnTargetCameraActive = false;
        lockOnTargetCamera.Priority = 1;

        UpdateCurrentActiveCamera();
    }

    private void IntroStarted()
    {
        introCamera.Priority = -1;
        normalCamera.Priority = 20;

        normalOrbitalFollow.HorizontalAxis.Value = 0;
        normalOrbitalFollow.VerticalAxis.Value = 17.5f;

        foreach (var c in normalAxisController.Controllers)
        {
            if (c.Name == "Look Orbit X")
            {
                c.Input.Gain = 0;
            }

            if (c.Name == "Look Orbit Y")
            {
                c.Input.Gain = 0;
            }
        }

        UpdateCurrentActiveCamera();
    }

    private void IntroFinished()
    {
        normalOrbitalFollow.HorizontalAxis.Value = 0;
        normalOrbitalFollow.VerticalAxis.Value = 17.5f;

        combatOrbitalFollow.HorizontalAxis.Value = normalOrbitalFollow.HorizontalAxis.Value;
        combatOrbitalFollow.VerticalAxis.Value = normalOrbitalFollow.VerticalAxis.Value;

        foreach (var c in normalAxisController.Controllers)
        {
            if (c.Name == "Look Orbit X")
            {
                c.Input.Gain = 1;
            }

            if (c.Name == "Look Orbit Y")
            {
                c.Input.Gain = -1;
            }
        }

        introFinished = true;

    }
}
