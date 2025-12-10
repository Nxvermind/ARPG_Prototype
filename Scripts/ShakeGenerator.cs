using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ShakeGenerator : MonoBehaviour
{
    public CinemachineBasicMultiChannelPerlin[] NoiseArray;

    public IEnumerator ApplyNoise(float amplitude, float frecuency, float duration)
    {
        foreach (var item in NoiseArray)
        {
            item.AmplitudeGain = amplitude;
            item.FrequencyGain = frecuency;
        }

        yield return new WaitForSecondsRealtime(duration);

        foreach (var item in NoiseArray)
        {
            item.AmplitudeGain = 0;
            item.FrequencyGain = 0;
        }
    }
}
