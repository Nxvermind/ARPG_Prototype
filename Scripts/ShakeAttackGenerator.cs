using Unity.Cinemachine;
using UnityEngine;

public class ShakeAttackGenerator : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {       
        EventBus.OnEnemyHitEvent += Shake;
    }

    private void OnDisable()
    {
        EventBus.OnEnemyHitEvent -= Shake;
    }   


    private void Shake()
    {
        float valueX = Random.value;
        float valueY = Random.value;

        float rndX = valueX > 0.5f ? -.2f : .2f;
        float rndY = valueY < 0.5f ? -.15f : .15f;

        Vector3 velocity = new(rndX, rndY, .1f);

        impulseSource.GenerateImpulseWithVelocity(velocity);
        //impulseSource.GenerateImpulse();
    }
}
