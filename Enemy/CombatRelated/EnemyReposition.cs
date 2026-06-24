using UnityEngine;
using UnityEngine.AI;

public class EnemyReposition : MonoBehaviour
{
    [SerializeField] private RepositionManager manager;
    [SerializeField] private MonoBehaviour movementProvider;
    private IPlayerMovementProvider provider;

    private EnemyCombatContext ctx;

    private NavMeshPath path;

    public Vector3 CurrentSlot { get; private set; }
    public bool HasSlot { get; private set; }

    [Header("Move Distance")]
    [SerializeField] private float minMoveDistance;

    [Header("Ideal Distance")]
    [SerializeField] private float minIdealDistance;
    [SerializeField] private float maxIdealDistance;

    [Header("Score Weights")]
    [SerializeField] private float distanceWeight;
    [SerializeField] private float angleWeight;
    [SerializeField] private float playerMovementWeight;
    
    private void Awake()
    {
        path = new();
        ctx = GetComponent<EnemyCombatContext>();

        provider = movementProvider as IPlayerMovementProvider;
    }

    private Vector3 GeneratePoint()
    {
        Vector3 pos;

        if (!manager.HasInnerRingSpace())
        {
            pos = GenerateRandomPointInRearArea();
            return pos;
        }

        if (manager.FrontSide.Count < manager.maxEnemiesInFrontSide || ctx.IsEnemyAlone)
        {
            pos = GenerateRandomPointInArea(-manager.frontSideAngle, manager.frontSideAngle);         
        }
        else
        {
            Vector3 dirToEnemy = (transform.position - ctx.cameraForwardReference.position).normalized;

            float angle = Vector3.SignedAngle(ctx.cameraForwardReference.forward, dirToEnemy, Vector3.up);

            if(angle > 0)
            {
                if (manager.RightSide.Count < manager.maxEnemiesInRightSide)
                {
                    pos = GenerateRandomPointInArea(manager.frontSideAngle, manager.rightSideAngle);
                }
                else
                {
                    pos = GenerateRandomPointInRearArea();
                }
            }
            else
            {
                if (manager.LeftSide.Count < manager.maxEnemiesInLeftSide)
                {
                    pos = GenerateRandomPointInArea(manager.leftSideAngle, -manager.frontSideAngle);
                }
                else
                {
                    pos = GenerateRandomPointInRearArea();
                }
            }
        }

        return pos;
    }
    private Vector3 GenerateRandomPointInArea(float minAngle, float maxAngle)
    {
        float randomAngle = Random.Range(minAngle, maxAngle);

        Vector3 randomDir = Quaternion.Euler(0, randomAngle, 0) * ctx.cameraForwardReference.forward;

        float t = Random.value;

        float radius = Mathf.Sqrt(Mathf.Lerp(minIdealDistance * minIdealDistance, maxIdealDistance * maxIdealDistance, t));

        Vector3 offset = randomDir * radius;

        return ctx.cameraForwardReference.position + offset;
    }
    private Vector3 GenerateRandomPointInRearArea()
    {
        bool preferIdeal = Random.value < .5f;
        float maxAngle = manager.rearSideAngle * (preferIdeal ? 0.8f : 1.0f);
        float randomAngle = Random.Range(-maxAngle, maxAngle);

        Vector3 randomDir = Quaternion.Euler(0, randomAngle, 0) * ctx.cameraForwardReference.forward;

        float t = Random.value;

        float radius = Mathf.Sqrt(Mathf.Lerp(maxIdealDistance * maxIdealDistance, manager.rearRadius * manager.rearRadius, t));

        Vector3 offset = randomDir * radius;

        return ctx.cameraForwardReference.position + offset;
    }
    private float CalculateScore(Vector3 point)
    {
        float score = 0f;

        score += GetPlayerMovementAlignmentScore(point);

        return score;
    }

    public bool TryGetDesiredPosition(out Vector3 bestPoint)
    {
        bestPoint = CurrentSlot;

        if (HasSlot && IsInIdealDistance(CurrentSlot)) return true;

        float bestScore = float.MinValue;

        Vector3 bestCandidate = CurrentSlot;
        bool foundBetter = false;

        for (int i = 0; i < 50; i++)
        {
            Vector3 candidate = GeneratePoint();

            if (!manager.IsSlotAvailable(candidate, this))
                continue;

            float sqrMoveDist = (candidate - transform.position).sqrMagnitude;
            float minMoveSqr = minMoveDistance * minMoveDistance;

            if (sqrMoveDist < minMoveSqr)
                continue;

            if (!NavMesh.CalculatePath(transform.position, candidate, NavMesh.AllAreas, path))
                continue;

            if (path.status != NavMeshPathStatus.PathComplete)
                continue;

            float score = CalculateScore(candidate);

            if (score > bestScore)
            {
                bestScore = score;
                bestCandidate = candidate;
                foundBetter = true;
            }
        }

        if (!foundBetter)
        {
            bestPoint = CurrentSlot;
            return false;
        }

        ReleaseSlot();

        CurrentSlot = bestCandidate;
        HasSlot = true;

        manager.Register(this);
        AssignToRespectiveSide(CurrentSlot);

        bestPoint = CurrentSlot;

        return true;
    }

    private float GetPlayerMovementAlignmentScore(Vector3 point)
    {
        Vector3 playerMoveDir = provider.GetMovement;

        if (playerMoveDir.sqrMagnitude < 0.01f)
            return 0; 

        playerMoveDir.Normalize();

        Vector3 toPoint = (point - ctx.cameraForwardReference.position).normalized;

        float dot = Vector3.Dot(playerMoveDir, toPoint);

        float normalizedDot = (dot + 1f) * 0.5f;

        return normalizedDot * playerMovementWeight;
    }

    private float ForwardAlignmentScore(Vector3 point)
    {
        Vector3 toPoint = point - ctx.cameraForwardReference.position;

        Vector3 dir = toPoint.normalized;

        float dot = Vector3.Dot(ctx.cameraForwardReference.forward, dir);

        float normalizedDot = (dot + 1f) * 0.5f;

        float noise = Random.Range(.7f, 1);

        noise = Mathf.Round(noise * 10f) / 10f;

        return noise * normalizedDot * angleWeight;
    }

    private bool IsInIdealDistance(Vector3 point)
    {
        Vector3 toPlayer = ctx.cameraForwardReference.position - point;

        float sqrDistance = toPlayer.sqrMagnitude;

        return sqrDistance >= minIdealDistance * minIdealDistance &&
               sqrDistance <= maxIdealDistance * maxIdealDistance;
    }

    public void ReleaseSlot()
    {
        if (!HasSlot)
            return;
        manager.RemoveFromSideSpace(this);

        HasSlot = false;
    }

    private void AssignToRespectiveSide(Vector3 point)
    {
        Vector3 dirToPoint = (point - ctx.cameraForwardReference.position).normalized;

        float angle = Vector3.SignedAngle(ctx.cameraForwardReference.forward, dirToPoint, Vector3.up);

        if(-manager.frontSideAngle < angle && angle < manager.frontSideAngle)
        {
            manager.AssignToFrontSide(this);
        }
        else if (manager.leftSideAngle < angle && angle < -manager.frontSideAngle)
        {
            manager.AssignToLeftSide(this);
        }
        else if(manager.frontSideAngle < angle && angle < manager.rightSideAngle)
        {
            manager.AssignToRightSide(this);
        }
    }

    private void OnDestroy()
    {
        ReleaseSlot();
    }
}
