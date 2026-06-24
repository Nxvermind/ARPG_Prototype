using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private LayerMask obstacleAndPlayerMask;
    [SerializeField] private LayerMask enemyBodyMask;

    [SerializeField] private float visionHeight;
    [SerializeField] private Vector3 halfExtents;

    [SerializeField] private float visionAngle;
    public float visionRange;
    [Space]
    [SerializeField] private float secondaryVisionAngle;
    [SerializeField] private float secondaryVisionRange;
    [Space]
    [SerializeField] private float obstacleVisionRange;
    [Space]
    public bool alreadySeenPlayer;

    public bool isPlayerInSight;

    public bool playerDisappeared;

    public bool wasPlayerDetected;

    private readonly RaycastHit[] hitsBuffer = new RaycastHit[20];

    public void CheckVision()
    {
        Vector3 ToPlayer = target.position - transform.position;
        float sqrDistance = ToPlayer.sqrMagnitude;

        float dot = DirectionUtility.Dot(transform.parent, target);

        float angle = Mathf.Cos(visionAngle * Mathf.Deg2Rad);
        float secVisionAngle = Mathf.Cos(secondaryVisionAngle * Mathf.Deg2Rad);

        float sqrVisionRange = visionRange * visionRange;

        float sqrSecondaryVisionRange = secondaryVisionRange * secondaryVisionRange;

        if (dot > angle && sqrDistance <= sqrVisionRange || dot > secVisionAngle && sqrDistance <= sqrSecondaryVisionRange)
        {
            if (IsVisionBlockedByObstacles())
            {
                isPlayerInSight = false;
            }
            else
            {
                isPlayerInSight = true;
                wasPlayerDetected = true;            
            }
        }
        else
        {
            isPlayerInSight = false;
        }
    }

    public bool IsVisionBlockedByEnemies(float distance = 15)
    {
        Vector3 origin = transform.position + Vector3.up * visionHeight;
        Vector3 targetPos = target.position + Vector3.up * visionHeight;
        Vector3 direction = (targetPos - origin).normalized;

        int count = Physics.SphereCastNonAlloc(origin, .7f, direction, hitsBuffer, distance, enemyBodyMask);

        for (int i = 0; i < count; i++)
        {
            if (hitsBuffer[i].transform == transform) continue;

            return true; 
        }

        return false;
    }
    private bool IsVisionBlockedByObstacles()
    {
        Vector3 origin = transform.position + transform.up * visionHeight;
        Vector3 targetPos = target.position + transform.up * visionHeight;

        Vector3 directionToPlayer = (targetPos - origin).normalized;

        Quaternion rot = Quaternion.LookRotation(directionToPlayer);

        if (Physics.BoxCast(origin, halfExtents, directionToPlayer, out RaycastHit hit, rot, visionRange, obstacleAndPlayerMask))
        {
            if (hit.transform == target.transform)
            {
                return false;
            }
            return true;
        }

        return false;
    }
}

