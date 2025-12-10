using System.Collections;
using UnityEngine;

public class EnemyAirGotHitState : EnemyGotHitState
{
    public EnemyAirGotHitState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        entity.Agent.enabled = false;
        entity.rb.isKinematic = true;

        if (entity.LastAttackNode.botStrongAttack)
        {
            animationHandler.Play("EmptyState", 0, 0f); // un state vacío en el animator
            animationHandler.Anim.Update(0);
            animationHandler.Play("AirGot_Hit_Up", 0, 0f); // vuelve a tu animación
        }
        else if (entity.LastAttackNode.upStrongAttack)
        {
            animationHandler.Play("EmptyState", 0, 0f); // un state vacío en el animator
            animationHandler.Anim.Update(0);
            animationHandler.Play("AirGot_Hit_Down",0,0);
        }

        entity.StartCoroutine(Cor());
    }

    public override void Exit()
    {
        base.Exit();
        entity.StopAllCoroutines();
        entity.rb.isKinematic = false;
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.NormalizedTime() >= .8f)
        {
            stateMachine.ChangeState(enemyStateFactory.FallingState);
        }
    }
    IEnumerator Cor()
    {
        yield return new WaitForSecondsRealtime(.1f);
        entity.rb.isKinematic = false;
    }

}
