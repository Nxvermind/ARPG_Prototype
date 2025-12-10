using System.Collections;
using UnityEngine;

public class HitstopManager : MonoBehaviour
{
    [SerializeField] private float duration;

    void OnEnable()
    {
        PlayerEvents.OnSuccessfulParryEvent += ApplyHitstopRoutine;
    }

    void OnDisable()
    {
        PlayerEvents.OnSuccessfulParryEvent -= ApplyHitstopRoutine;
    }


    public void ApplyHitstopRoutine()
    {
        TimeScaleManager.Instance.ApplyHitstop(0.1f, duration);
    }
}
