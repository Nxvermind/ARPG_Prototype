using Unity.Cinemachine;
using UnityEngine;

public class ParryShakeGenerator : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        PlayerEvents.OnSuccessfulParryEvent += Shake;
    }

    void OnDisable()
    {
        PlayerEvents.OnSuccessfulParryEvent -= Shake;
    }

    private void Shake()
    {
        float rnd = Random.Range(-1f, 1f);

        Vector2 vel = rnd switch
        {
            > 0f and <= 0.5f => new Vector2(0.28f, 0.2f),

            > 0.5f => new Vector2(-0.28f, 0.2f),

            < 0f and >= -0.5f => new Vector2(0.28f, -0.2f),

            < -0.5f => new Vector2(-0.28f, -0.2f),

            _ => Vector2.zero
        };

        impulseSource.GenerateImpulseWithVelocity(vel);
    }
}
