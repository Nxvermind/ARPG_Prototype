using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class EnemyCombatSystem : MonoBehaviour
{
    [SerializeField] private float rangeToAllowNextAttack;
    public EnemyAttackSettings attackSettings;
    [Space]
    public bool hasAggresiveSettings;
    [ShowIf("hasAggresiveSettings")]
    public EnemyAttackSettings aggresiveSettings;

    public EnemyAttackData CurrentAttackData { get; private set; }

    private bool comboEnabled;

    private int currentComboData;
    private int comboIndex = 0;

    private AnimationHandler animationHandler;

    private Enemy enemy;

    [HideInInspector] public bool canPlayNextAttack;

    private Coroutine comboCoroutine;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
        enemy = GetComponent<Enemy>();
    }

    public void ResetCombo()
    {

        comboEnabled = false;
        currentComboData = 0;
        comboIndex = 0;

        if(comboCoroutine != null) StopCoroutine(comboCoroutine);
    }

    public EnemyAttackData GetRandomBasicAttack()
    {
        int index = Random.Range(0, attackSettings.basicLightAttack.Length);

        return attackSettings.basicLightAttack[index];
    }

    public void GetRandomAttack()
    {
        if(attackSettings.comboAttackDatas?.Length > 0)
        {
            int rnd = Random.Range(1, 101);

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

                    return;
                }
            }
        }

        comboEnabled = false;
        CurrentAttackData = GetRandomBasicAttack();
    }

    public void ExecuteAttack()
    {
        if (comboEnabled)
        {
            comboCoroutine = StartCoroutine(ComboCoroutine());
        }
        else
        {
            PlayAttack(CurrentAttackData);
        }
    }

    private void PlayAttack(EnemyAttackData _currentAttackData)
    {
        if(enemy.EnemyHitBox != null)
        {
            enemy.EnemyHitBox.HitData = _currentAttackData.hitData;
        }

        animationHandler.CrossFade(_currentAttackData.attackAnimationName, 0.1f);
    }

    //Called in an animation Event
    public void AllowNextAttack()
    {
        if (!enemy.IsPlayerInRange(rangeToAllowNextAttack) || !enemy.EnemyVision.isPlayerInSight) return;

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
        var combo = attackSettings.comboAttackDatas[currentComboData].comboAttack;

        while (comboIndex < combo.Length)
        {
            CurrentAttackData = combo[comboIndex];
            PlayAttack(CurrentAttackData);

            yield return new WaitUntil(() =>  animationHandler.IsPlaying(CurrentAttackData.attackAnimationName) && 
            animationHandler.NormalizedTime() >= 0.3f && canPlayNextAttack);

            comboIndex++;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.white;

    //    Gizmos.DrawWireSphere(transform.position, rangeToAllowNextAttack);
    //}
}
