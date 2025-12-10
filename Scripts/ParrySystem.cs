using System.Collections;
using UnityEngine;

public class ParrySystem : MonoBehaviour, IHitable
{
    private PlayerModel playerModel;

    private PlayerParameters playerParameters;

    [SerializeField] private InputHandler inputHandler;

    [SerializeField] private float parryWindow = 0.2f;

    [SerializeField] private float timeToParryAttack; //if player attack after a successfull parry then execute the parry attack instead the normal attack

    public BoxCollider coll;

    public bool canParryAttack;

    public bool isParryActive; //called on the parrySystem script

    public bool isPlayerBlocking;

    public bool gotHitWhileBlocking;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
        playerParameters = GetComponentInParent<PlayerParameters>();

        playerModel = new PlayerModel(playerParameters);
    }

    private IEnumerator ParryAttackCor()
    {
        canParryAttack = true;
        yield return new WaitForSecondsRealtime(timeToParryAttack);
        canParryAttack = false;

    }

    private void SuccessfullParry(Enemy attacker)
    {
        attacker.EnemyParryStatus.isParried = true;
        attacker.EnemyCombatSystem.ResetCombo();
    }

    //Called in an AnimationEvent
    public IEnumerator ParryWindowRoutine()
    {
        isParryActive = true;
        yield return new WaitForSecondsRealtime(parryWindow);
        isParryActive = false;
    }

    public void OnHit(Enemy attacker)
    {
        if (isParryActive)
        {
            SuccessfullParry(attacker);
            StartCoroutine(ParryAttackCor());

            PlayerEvents.SuccessfulParryEvent();
        }
        else
        {
            playerParameters.currentPostureValue -= attacker.EnemyCombatSystem.CurrentAttackData.postureDamage;
            playerModel.UpdatePostureValue(attacker.EnemyCombatSystem.CurrentAttackData.postureDamage);
            PlayerEvents.PlayerBlockEvent();
        }
    }
}
