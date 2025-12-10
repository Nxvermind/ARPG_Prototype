using UnityEngine;

public class ExecutionSpline : MonoBehaviour
{

    public Transform parent;

    private void OnEnable()
    {
        EventBus.OnExecutionStarted += SetParentToSpline;
    }

    private void OnDisable()
    {
        EventBus.OnExecutionStarted -= SetParentToSpline;
    }

    private void SetParentToSpline()
    {
        gameObject.transform.SetPositionAndRotation(parent.position, parent.rotation);
    }
}
