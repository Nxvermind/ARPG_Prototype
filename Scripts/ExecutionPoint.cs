using System.Collections;
using UnityEngine;

public class ExecutionPoint : MonoBehaviour
{
    public Transform player;

    private void OnEnable()
    {
        EventBus.OnExecutionStarted += SetPos;
    }

    private void OnDisable()
    {
        EventBus.OnExecutionStarted -= SetPos;
    }

    private void SetPos()
    {
        StartCoroutine(Cor());
    }

    IEnumerator Cor()
    {
        gameObject.transform.SetParent(player);
        yield return new WaitForSecondsRealtime(1.9f);
        gameObject.transform.SetParent(null);
    }

}
