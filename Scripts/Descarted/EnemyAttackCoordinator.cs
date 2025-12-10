using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCoordinator : MonoBehaviour
{
    [Tooltip("if player is executing an enemy, nobody is allowed to attack for this time")]
    [SerializeField] private float executionTime;

    public List<Enemy> enemyAttackWaitList = new();

    public List<Enemy> meleeWaitList = new();

    public List<Enemy> rangeWaitList = new();

    public Enemy currentActiveAttacker;

    public Enemy currentRangedActiveAttacker;

    private Enemy lastActiveAttacker;

    private Enemy lastRangedActiveAttacker;

    #region Check Active Attacker

    private float checkInterval = 4;

    private float checkTimer;

    private bool activeAttackerDetected = true;

    #endregion

    //TODO

    //quiero que los enemigos se registren para obtener su combatStatus. El primer enemigo que ve al player tiene el permiso para atacar.                               | Done
    //Cuando ataca deja de ser el active attacker y se le da permiso al enemigo que paso mas tiempo sin atacar.                                                         | Done
    //si el active attacker no ataca por mas de 5 segundos (sea por el motivo que sea) le da permiso al enemigo que paso mas tiempo sin atacar.                         | Done
    //los enemigos de rango tienen permiso para atacar que se activa cada 4 segundos y solo uno puede atacar a la vez.                                                  | Done
    //si el player está cerca de un enemigo melee (por mas de 3s) que no tiene permiso para atacar ese enemigo obtiene permiso aunque ya haya otro activeAttacker       | Done
    //si el player está ejecutando a un enemigo entonces nadie tiene permiso para atacar así sea un active attacker                                                     | Done

    private void Start()
    {
        checkTimer = checkInterval;
    }

    private void OnEnable()
    {
        EventBus.OnEnemyDeathEvent += UnregisterEnemy;
        EventBus.OnEnemyDeathEvent += OnEnemyDeath;
        EventBus.OnExecutionStarted += NobodyIsAllowedToAttack;
    }

    private void OnDisable()
    {
        EventBus.OnEnemyDeathEvent -= UnregisterEnemy;
        EventBus.OnEnemyDeathEvent -= OnEnemyDeath;
        EventBus.OnExecutionStarted -= NobodyIsAllowedToAttack;
    }

    private void Update()
    {
        if (meleeWaitList.Count <= 1) return;

        CheckMeleeActiveAttacker();
    }

    public void RegisterEnemy(Enemy enemy)
    {
        enemyAttackWaitList.Add(enemy);

        if(enemy.enemyType == EnemyType.Melee)
        {
            meleeWaitList.Add(enemy);
        }
        else if(enemy.enemyType == EnemyType.Range)
        {
            rangeWaitList.Add(enemy);
        }

        //Debug.Log($"{enemy.name} registered and its type is {enemy.enemyType}");
    }

    private void UnregisterEnemy(Enemy enemy)
    {
        enemyAttackWaitList.Remove(enemy);

        if (enemy.enemyType == EnemyType.Melee)
        {
            meleeWaitList.Remove(enemy);
        }
        else if (enemy.enemyType == EnemyType.Range)
        {
            rangeWaitList.Remove(enemy);
        }

        //Debug.Log($"{enemy.name} is unregistered");
    }

    public void PermissionToBeActiveAttacker(Enemy enemy)
    {
        if(enemy.enemyType == EnemyType.Melee)
        {
            if (currentActiveAttacker == null)
            {
                currentActiveAttacker = enemy;

                currentActiveAttacker.EnemyCombatStatus.waitingForAttackPermission = false;

                currentActiveAttacker.EnemyCombatStatus.isActiveAttacker = true;

            }
            else
            {
                enemy.EnemyCombatStatus.waitingForAttackPermission = true;
            }
        }
        else if(enemy.enemyType == EnemyType.Range)
        {
            if (currentRangedActiveAttacker == null)
            {
                currentRangedActiveAttacker = enemy;

                currentRangedActiveAttacker.EnemyCombatStatus.waitingForAttackPermission = false;

                currentRangedActiveAttacker.EnemyCombatStatus.isActiveAttacker = true;
            }
            else
            {
                enemy.EnemyCombatStatus.waitingForAttackPermission = true;
            }
        }
    }

    public void GrantMeleeAttackerPermissionToAttack(Enemy enemy)
    {
        if (!enemy.EnemyCombatStatus.isAllowedToAttack) return;

        if(enemy.enemyType == EnemyType.Melee)
        {
            enemy.EnemyCombatStatus.oneTimeAttackPermission = true;

            enemy.EnemyCombatStatus.waitingForAttackPermission = false;

            enemy.EnemyCombatStatus.isActiveAttacker = true;


            enemy.EnemyCombatStatus.timeSinceWaitingForAttackPermission = 0;
        }

    }

    public void ChangeActiveAttacker(Enemy enemy)
    {
        if (enemy.enemyType == EnemyType.Melee)
        {
            ChangeCurrentActiveAttacker(meleeWaitList);
        }
        else if (enemy.enemyType == EnemyType.Range)
        {
            ChangeCurrentActiveAttacker(rangeWaitList);
        }
    }

    private void ChangeCurrentActiveAttacker(List<Enemy> enemyList)
    {
        if(enemyList == meleeWaitList)
        {
            SetLastActiveAttacker(meleeWaitList);

            float highestValue = float.MinValue;

            foreach (Enemy enemy in enemyList)
            {
                if (!enemy.EnemyCombatStatus.waitingForAttackPermission) continue;

                if (enemy.EnemyCombatStatus.timeSinceWaitingForAttackPermission > highestValue)
                {
                    highestValue = enemy.EnemyCombatStatus.timeSinceWaitingForAttackPermission;
                    currentActiveAttacker = enemy;
                }
            }

            currentActiveAttacker.EnemyCombatStatus.waitingForAttackPermission = false;
            currentActiveAttacker.EnemyCombatStatus.isActiveAttacker = true;

        }
        else if(enemyList == rangeWaitList)
        {
            SetLastActiveAttacker(rangeWaitList);

            float highestValue = float.MinValue;

            foreach (Enemy enemy in enemyList)
            {
                if (!enemy.EnemyCombatStatus.waitingForAttackPermission) continue;

                if (enemy.EnemyCombatStatus.timeSinceWaitingForAttackPermission > highestValue)
                {
                    highestValue = enemy.EnemyCombatStatus.timeSinceWaitingForAttackPermission;
                    currentRangedActiveAttacker = enemy;
                }
            }

            currentRangedActiveAttacker.EnemyCombatStatus.waitingForAttackPermission = false;
            currentRangedActiveAttacker.EnemyCombatStatus.isActiveAttacker = true;

            StartRangeCooldown(currentRangedActiveAttacker);
        }
    }

    private void StartRangeCooldown(Enemy enemy)
    {
        enemy.EnemyCombatStatus.readyToAttack = false;
        StartCoroutine(RangeCooldownCor(enemy));
    }

    IEnumerator RangeCooldownCor(Enemy enemy)
    {
        yield return new WaitForSecondsRealtime(enemy.EnemyCombatStatus.timeToLetRangeEnemyAttack);
        enemy.EnemyCombatStatus.readyToAttack = true;
    }


    private void SetLastActiveAttacker(List<Enemy> enemyList)
    {
        if (enemyList == meleeWaitList)
        {
            if (currentActiveAttacker == null) return;

            lastActiveAttacker = currentActiveAttacker;

            lastActiveAttacker.EnemyCombatStatus.isActiveAttacker = false;
            lastActiveAttacker.EnemyCombatStatus.waitingForAttackPermission = true;
            lastActiveAttacker.EnemyCombatStatus.timeSinceWaitingForAttackPermission = 0;
        }
        else if (enemyList == rangeWaitList)
        {
            if (currentRangedActiveAttacker == null) return;

            lastRangedActiveAttacker = currentRangedActiveAttacker;

            lastRangedActiveAttacker.EnemyCombatStatus.isActiveAttacker = false;
            lastRangedActiveAttacker.EnemyCombatStatus.waitingForAttackPermission = true;
            lastRangedActiveAttacker.EnemyCombatStatus.timeSinceWaitingForAttackPermission = 0;
        }
    }

    public void ResetAttackPermission(Enemy enemy)
    {
        enemy.EnemyCombatStatus.oneTimeAttackPermission = false;

        enemy.EnemyCombatStatus.isActiveAttacker = false;

        enemy.EnemyCombatStatus.waitingForAttackPermission = true;

        enemy.EnemyCombatStatus.timeSinceWaitingForAttackPermission = 0;

        if (enemy.enemyType == EnemyType.Range)
        {
            StartRangeCooldown(enemy);
        }
    }

    private void CheckMeleeActiveAttacker()
    {
        if (currentActiveAttacker == null || meleeWaitList.Count <= 1) return;

        checkTimer -= Time.deltaTime;

        if (checkTimer >= 0) return;

        activeAttackerDetected = false; 

        foreach (var enemy in meleeWaitList)
        {
            if (enemy.EnemyCombatStatus.isActiveAttacker)
            {
                activeAttackerDetected = true;
                break;
            }
        }

        if (!activeAttackerDetected)
        {
            Debug.Log("no active attacker detected");
            ChangeCurrentActiveAttacker(meleeWaitList);
        }

        checkTimer = checkInterval;
    }

    private void NobodyIsAllowedToAttack()
    {
        StartCoroutine(NobodyIsAllowedToAttackCor());
    }

    IEnumerator NobodyIsAllowedToAttackCor()
    {
        foreach (var enemy in enemyAttackWaitList)
        {
            enemy.EnemyCombatStatus.isAllowedToAttack = false;
        }

        yield return new WaitForSecondsRealtime(executionTime);

        foreach (var enemy in enemyAttackWaitList)
        {
            enemy.EnemyCombatStatus.isAllowedToAttack = true;
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        enemy.EnemyCombatStatus.isActiveAttacker = false;
        enemy.EnemyCombatStatus.waitingForAttackPermission = false;

        enemy.EnemyCombatStatus.timeSinceWaitingForAttackPermission = 0;

        if (currentActiveAttacker == enemy || currentRangedActiveAttacker == enemy)
        {
            currentActiveAttacker = null;

            if (enemyAttackWaitList.Count > 1)
            {
                ChangeActiveAttacker(enemy);
            }
        }


        if (enemyAttackWaitList.Count == 0)
        {
            currentActiveAttacker = null;
            currentRangedActiveAttacker = null;
        }
    }
}
