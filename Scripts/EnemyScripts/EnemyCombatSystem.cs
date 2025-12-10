using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class EnemyCombatSystem : MonoBehaviour
{
    public EnemyAttackSettings attackSettings;
    [Space]
    public bool hasAggresiveSettings;
    [ShowIf("hasAggresiveSettings")]
    public EnemyAttackSettings aggresiveSettings;
    public EnemyAttackData CurrentAttackData { get; private set; }

    private int currentComboData;
    private int comboIndex = 0;

    private bool comboEnabled;

    private AnimationHandler animationHandler;

    private Enemy enemy;

    [HideInInspector] public bool canPlayNextAttack;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
        enemy = GetComponent<Enemy>();
    }

    public void ResetCombo()
    {
        currentComboData = 0;
        comboIndex = 0;
        StopAllCoroutines();
    }

    public EnemyAttackData GetRandomAttack()
    {
        comboEnabled = false;

        if(attackSettings.comboAttackDatas != null)
        {
            float rnd = Random.Range(0, 100);

            float cumulative = 0;

            for (int i = 0; i < attackSettings.comboAttackDatas.Length; i++)
            {
                cumulative += attackSettings.comboAttackDatas[i].probabilityToActivate;

                if (rnd <= cumulative)
                {
                    comboIndex = 0;
                    currentComboData = i;
                    CurrentAttackData = attackSettings.comboAttackDatas[i].comboAttack[comboIndex];
                    comboEnabled = true;

                    return CurrentAttackData;
                }
            }
        }

        return attackSettings.GetRandomBasicAttack();

    }

    public void ExecuteAttack(EnemyAttackData _attackData)
    {
        CurrentAttackData = _attackData;

        if (comboEnabled)
        {
            StartCoroutine(ComboCoroutine());
        }
        else
        {
            PlayAttack(CurrentAttackData);
        }
    }

    private void PlayAttack(EnemyAttackData _currentAttackData)
    {
        animationHandler.CrossFade(_currentAttackData.attackAnimationName, 0.1f);
    }

    //Called in an animation Event
    public void AllowNextAttack()
    {
        if (!enemy.IsPlayerInRange(5.5f))
        {
            canPlayNextAttack = false;
            return;
        }

        StartCoroutine(AllowNextAttackCor());
    }

    private IEnumerator AllowNextAttackCor()
    {
        canPlayNextAttack = true;
        yield return null;
        canPlayNextAttack = false;
    }

    private IEnumerator ComboCoroutine()
    {
        while (comboIndex < attackSettings.comboAttackDatas[currentComboData].comboAttack.Length)
        {
            PlayAttack(CurrentAttackData);

            yield return new WaitUntil(() => animationHandler.IsPlaying(CurrentAttackData.attackAnimationName) && animationHandler.NormalizedTime() >= .3f && canPlayNextAttack);

            comboIndex++;

            if (comboIndex < attackSettings.comboAttackDatas[currentComboData].comboAttack.Length)
            {
                CurrentAttackData = attackSettings.comboAttackDatas[currentComboData].comboAttack[comboIndex];
            }
        }
    }
}
