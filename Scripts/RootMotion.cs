using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MonoBehaviour
{
    private Player player;

    private EnemyDetector enemyDetector;

    private AnimationHandler animationHandler;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        animationHandler = GetComponent<AnimationHandler>();
        enemyDetector = transform.parent.GetComponentInChildren<EnemyDetector>();
    }

    private void Update()
    {
        if (enemyDetector.IsCurrentTargetClose(1.5f))
        {
            if (!player.PlayerBlackboard.isAttacking) return;


            animationHandler.Anim.applyRootMotion = false;
        }
    }

    private void OnAnimatorMove()
    {
        if (animationHandler.Anim.applyRootMotion)
        {
            Vector3 delta = animationHandler.Anim.deltaPosition;

            player.CharacterController.Move(delta);

            player.transform.rotation *= animationHandler.Anim.deltaRotation;
        }
    }

    public void RootMotionOn()
    {
        animationHandler.Anim.applyRootMotion = true;
    }

    public void RootMotionOff()
    {
        animationHandler.Anim.applyRootMotion = false;
    }
}
