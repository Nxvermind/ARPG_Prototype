using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private LayerMask layerMask;

    public float visionHeight;

    [SerializeField] private float visionAngle;
    public float visionRange;
    [Space]
    [SerializeField] private float secondaryVisionAngle;
    [SerializeField] private float secondaryVisionRange;
    [Space]
    [SerializeField] private float obstacleVisionRange;
    [Space]
    [Tooltip("This should be false if its the first time on playmode. Dont forget it")]
    public bool alreadySeenPlayer;

    public bool playerInSight;

    public bool playerDisappeared;

    public bool wasPlayerDetected;

    void Update()
    {
        DetectPlayerInVisionAngle();
    }

    private void DetectPlayerInVisionAngle()
    {
        Vector3 DistanceToPlayer = target.position - transform.position;

        float angle = Vector3.Angle(transform.forward, DistanceToPlayer.normalized);

        float sqrDistance = DistanceToPlayer.sqrMagnitude;

        float sqrVisionRange = visionRange * visionRange;

        float sqrSecondaryVisionRange = secondaryVisionRange * secondaryVisionRange;

        if (angle < visionAngle && sqrDistance <= sqrVisionRange || angle < secondaryVisionAngle && sqrDistance <= sqrSecondaryVisionRange)
        {
            wasPlayerDetected = true;

            Vector3 origin = transform.position + Vector3.up * visionHeight;

            Vector3 directionToPlayer = (target.position - transform.position).normalized;

            if (Physics.Linecast(origin, origin + directionToPlayer * visionRange, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Obstacle"))
                {
                    playerInSight = false;
                }
                else
                {
                    playerInSight = true;
                }
            }
        }
        else
        {
            playerInSight = false;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;

    //    Vector3 leftDir = Quaternion.Euler(0, -visionAngle, 0) * transform.forward;

    //    Gizmos.DrawLine(transform.position, transform.position + leftDir * visionRange);

    //    Vector3 rightDir = Quaternion.Euler(0, visionAngle, 0) * transform.forward;

    //    Gizmos.DrawLine(transform.position, transform.position + rightDir * visionRange);

    //    Gizmos.color = Color.red;

    //    Vector3 secondaryLeftDir = Quaternion.Euler(0, -secondaryVisionAngle, 0) * transform.forward;

    //    Gizmos.DrawLine(transform.position, transform.position + secondaryLeftDir * secondaryVisionRange);

    //    Vector3 secondaryRightDir = Quaternion.Euler(0, secondaryVisionAngle, 0) * transform.forward;

    //    Gizmos.DrawLine(transform.position, transform.position + secondaryRightDir * secondaryVisionRange);

    //    Vector3 origin = transform.position + Vector3.up * visionHeight;

    //    Vector3 directionToPlayer = (target.position - transform.position).normalized;

    //    Gizmos.color = Color.red;

    //    Gizmos.DrawLine(origin, origin + directionToPlayer * visionRange);
    //}
}
