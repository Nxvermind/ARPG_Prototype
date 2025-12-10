using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class ExecutionCamera : MonoBehaviour
{
    private CinemachineSplineDolly splineDolly;

    [SerializeField] private float executionDuration;

    public bool ExecutionFinished { get; private set; }

    private void Awake()
    {
        splineDolly = GetComponent<CinemachineSplineDolly>();        
    }

    void Start()
    { 
        EventBus.OnExecutionStarted += HandleExecution;
    }

    private void OnDestroy()
    {
        EventBus.OnExecutionStarted -= HandleExecution;
    }

    private void HandleExecution()
    {
        StartCoroutine(ExecutionCor());
    }

    IEnumerator ExecutionCor()
    {
        ExecutionFinished = false;

        float time = 0;

        float startPos = 0;

        float endPos = 1;

        splineDolly.SplineSettings.Position = 0;

        while (time < executionDuration)
        {
            time += Time.deltaTime;
            splineDolly.SplineSettings.Position = Mathf.Lerp(startPos, endPos, time / executionDuration);
            yield return null;
        }

        splineDolly.SplineSettings.Position = endPos;
        ExecutionFinished = true;
    }
}
