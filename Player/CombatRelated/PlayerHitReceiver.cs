using UnityEngine;

public class PlayerHitReceiver : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void ReceiveHit(HitData hitData, Enemy attacker)
    {
        if (player.PlayerBlackboard.isInvulnerable) return;

        if (ResolveDefense(hitData, attacker)) return;

        TakeDamage(hitData.damage);

        if (!CanReactToHit()) return;

        ReactToHit();
    }

    private bool ResolveDefense(HitData hitData, Enemy attacker)
    {
        if (!player.ParrySystem.isBlocking) return false;

        if (hitData.ignoreBlocking) return false;

        float dot = DirectionUtility.Dot(transform, attacker.transform);

        if (dot <= .7f) return false;

        Debug.Log("Defense resolved");

        if (player.ParrySystem.isParryActive && hitData.isParryable)
        {
            player.ParrySystem.SuccessfulParry(attacker);
            PlayerEvents.SuccessfulParryEvent();
        }
        else
        {
            player.PlayerModel.DecreasePostureValue(hitData.postureDamage);
            player.AnimationHandler.Play("Parry_Accept");
        }

        return true;
    }

    private bool CanReactToHit()
    {
        return !player.PlayerBlackboard.onlyTakeDamage;
    }

    private void ReactToHit()
    {
        player.StateMachine.UnlockState();
        player.StateMachine.ChangeState(player.PlayerStateFactory.GotHitState);
    }

    private void TakeDamage(float damage)
    {
        player.PlayerModel.DecreaseCurrentHP(damage);
    }
}
