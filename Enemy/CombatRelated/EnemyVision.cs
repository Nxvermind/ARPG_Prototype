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

    //private RaycastHit lastHit;
    //private bool hasHit;

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
            //hasHit = true;
            //lastHit = hit;

            if (hit.transform == target.transform)
            {
                return false;
            }

            //Debug.Log($"vision blocked by {hit.transform.name}, player is NOT in sight");
            return true;
        }

        //hasHit = false;
        return false;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    // Campo de visión
    //    Gizmos.color = Color.white;

    //    Vector3 right = Quaternion.Euler(0, visionAngle, 0) * transform.forward;
    //    Vector3 left = Quaternion.Euler(0, -visionAngle, 0) * transform.forward;

    //    Gizmos.DrawLine(transform.position,
    //        transform.position + right * visionRange);

    //    Gizmos.DrawLine(transform.position,
    //        transform.position + left * visionRange);

    //    Vector3 dir = (target.position - transform.position).normalized;
    //    Vector3 toPlayer = transform.position + transform.up * visionHeight + dir * visionRange;

    //    Gizmos.DrawLine(transform.position + transform.up * visionHeight, toPlayer);

    //    //----------------------------------------
    //    // BoxCast
    //    //----------------------------------------

    //    //Vector3 origin = transform.position + transform.up * visionHeight;
    //    //Vector3 targetPos = target.position + transform.up * visionHeight;

    //    //Vector3 direction = (targetPos - origin).normalized;

    //    //Quaternion rot = Quaternion.LookRotation(direction);

    //    //float distance = hasHit ? lastHit.distance : visionRange;

    //    //Vector3 endPos = origin + direction * distance;

    //    //// Línea central

    //    //Gizmos.color = Color.yellow;
    //    //Gizmos.DrawLine(origin, endPos);

    //    //Matrix4x4 oldMatrix = Gizmos.matrix;

    //    //// Caja inicial

    //    //Gizmos.color = Color.green;

    //    //Gizmos.matrix = Matrix4x4.TRS(
    //    //    origin,
    //    //    rot,
    //    //    Vector3.one);

    //    //Gizmos.DrawWireCube(
    //    //    Vector3.zero,
    //    //    halfExtents * 2f);

    //    //// Caja final

    //    //Gizmos.color = hasHit ? Color.red : Color.cyan;

    //    //Gizmos.matrix = Matrix4x4.TRS(
    //    //    endPos,
    //    //    rot,
    //    //    Vector3.one);

    //    //Gizmos.DrawWireCube(
    //    //    Vector3.zero,
    //    //    halfExtents * 2f);

    //    //Gizmos.matrix = oldMatrix;
    //}
}

