using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    [SerializeField] private EnemyDetector enemyDetector;
    [SerializeField] private PlayerCombatContext ctx;
    [SerializeField] private LockOnTargetBlackboard lockOnTargetBlackboard; 

    public Transform CurrentTarget { get; private set; }
    public Transform currentSkillTarget;

    public Transform LockedTarget;
    public bool IsCurrentTargetLocked;

    public float interval;
    public float timer;

    [Header("To Find Enemies")]
    public float visionAngle;

    public float visionSkillAngle;

    public float visionRange;

    private void OnEnable()
    {
        EventBus.OnEnemyDeathEvent += OnEnemyDeath;
    }

    private void OnDisable()
    {
        EventBus.OnEnemyDeathEvent -= OnEnemyDeath;
    }

    public void UpdateTarget()
    {
        if (enemyDetector.NumOfEnemiesDetected == 0)
        {
            CurrentTarget = null;
            currentSkillTarget = null;
            return;
        }

        if (lockOnTargetBlackboard.IsLockOnTargetActive)
        {
            CurrentTarget = LockOnTargetLogic.CurrentLockOnTarget;
            currentSkillTarget = LockOnTargetLogic.CurrentLockOnTarget;

            return;
        }

        if (timer <= 0)
        {
            timer = interval;

            if (enemyDetector.NumOfEnemiesDetected <= 1)
            {
                currentSkillTarget = FindBestTargetForTeleportSkill(visionSkillAngle * 2);
            }
            else
            {
                currentSkillTarget = FindBestTargetForTeleportSkill(visionSkillAngle);
            }
            ResolveTarget();
        }

        timer -= Time.deltaTime;
        }

    private void ResolveTarget()
    {
        if (enemyDetector.NumOfEnemiesDetected == 1)
        {
            CurrentTarget = FindClosestEnemy(transform);
        }
        else if (enemyDetector.NumOfEnemiesDetected > 1)
        {
            CurrentTarget = FindBestTargetInVisionAngle(visionAngle);

            if(CurrentTarget == null)
            {
                CurrentTarget = FindClosestEnemy(transform);
            }

            if (IsCurrentTargetLocked)
            {
                if(CurrentTarget != null && CurrentTarget != LockedTarget && IsTargetInRange(4, CurrentTarget))
                {
                    UnlockedTarget();
                }
            }
        }
    }
    public void LockTarget()
    {
        if (CurrentTarget == null) return;

        LockedTarget = CurrentTarget;
        IsCurrentTargetLocked = true;
    }

    public void UnlockTarget()
    {
        IsCurrentTargetLocked = false;
        LockedTarget = null;
    }

    public bool IsTargetInRange(float distanceToTarget, Transform target)
    {
        if (target == null) return false;

        Vector3 targetPos = target.position;
        targetPos.y = 0.1f;

        Vector3 distance = targetPos - transform.position;

        float sqrDistance = distance.sqrMagnitude;

        float sqrDistanceToPlayer = distanceToTarget * distanceToTarget;

        return sqrDistance <= sqrDistanceToPlayer;
    }

    public Transform FindClosestEnemy(Transform origin)
    {
        Transform closestEnemy = null;

        float closestDistanceSqr = Mathf.Infinity;

        foreach (Transform enemy in enemyDetector.EnemiesDetected)
        {
            Vector3 dirToEnemy = (enemy.position - origin.position).normalized;

            float sqrDistance = (enemy.position - origin.position).sqrMagnitude;

            if (sqrDistance < closestDistanceSqr)
            {
                closestDistanceSqr = sqrDistance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    public Transform FindClosestEnemyForLockOn(Vector3 origin, Transform lockOnTarget)
    {
        Transform closestEnemy = null;

        float closestDistanceSqr = Mathf.Infinity;

        foreach (Transform enemy in enemyDetector.EnemiesDetected)
        {
            if (enemy == lockOnTarget) continue;

            Vector3 dirToEnemy = (enemy.position - origin).normalized;

            float sqrDistance = (enemy.position - origin).sqrMagnitude;

            if (sqrDistance < closestDistanceSqr)
            {
                closestDistanceSqr = sqrDistance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    public Transform FindBestTargetInVisionAngle(float visionAngle)
    {
        Transform bestTarget = null;

        Vector3 origin = ctx.CameraForward.position;
        Vector3 forward = ctx.CameraForward.forward;

        float cosThreshold = Mathf.Cos(visionAngle * Mathf.Deg2Rad);

        float bestScore = float.MinValue;

        foreach (Transform enemy in enemyDetector.EnemiesDetected)
        {
            Vector3 dirToEnemy = (enemy.position - origin).normalized;

            float dot = Vector3.Dot(forward, dirToEnemy);
            if (dot < cosThreshold) continue;

            float distance = Vector3.Distance(origin, enemy.position);

            float distanceScore = 1f - (distance / visionRange);

            float score = dot * 0.8f + distanceScore * 0.4f;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    public Transform FindBestTargetForTeleportSkill(float visionAngle)
    {
        Transform bestTarget = null;

        Vector3 origin = ctx.CameraForward.position;
        Vector3 forward = ctx.CameraForward.forward;

        float cosThreshold = Mathf.Cos(visionAngle * Mathf.Deg2Rad);

        float bestScore = float.MinValue;

        foreach (Transform enemy in enemyDetector.EnemiesDetected)
        {
            if (enemy.parent.position.y > 0) continue;

            Vector3 dirToEnemy = (enemy.position - origin).normalized;

            float dot = Vector3.Dot(forward, dirToEnemy);
            if (dot < cosThreshold) continue;

            float distance = Vector3.Distance(origin, enemy.position);

            float distanceScore = 1f - (distance / visionRange);

            float score = dot * 0.8f + distanceScore * 0.4f;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    public Transform FindClosestEnemyInVisionAngleFromPosition(Vector3 origin, Vector3 forward, float angle)
    {
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemyDetector.EnemiesDetected)
        {
            if (enemy == null) continue;

            Vector3 dirToEnemy = (enemy.position - origin).normalized;
            float distToEnemy = Vector3.Distance(origin, enemy.position);
            float angleToEnemy = Vector3.Angle(forward, dirToEnemy);

            if (angleToEnemy <= angle)
            {
                if (distToEnemy < minDistance)
                {
                    minDistance = distToEnemy;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        UnlockedTarget();
    }
}
