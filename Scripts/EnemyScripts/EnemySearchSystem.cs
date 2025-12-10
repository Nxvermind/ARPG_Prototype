using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySearchSystem : MonoBehaviour
{
    public int maxTimesToTry;
    public int currentTry;

    public float height;
    public float obstacleDistance;
    public LayerMask obstacleMask;

    public bool GetReachableRandomPosition(Vector3 initialPoint, NavMeshAgent agent , float radius, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;

            float minDistance = Mathf.Max(1f, radius * 0.5f);

            float randomDistance = Random.Range(minDistance, radius);

            Vector3 offset = new Vector3(randomDir.x, 0, randomDir.y) * randomDistance;

            Vector3 destiny = initialPoint + offset;

            if (NavMesh.SamplePosition(destiny, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                NavMeshPath path = new();

                agent.CalculatePath(hit.position, path);

                // Solo seguimos si el path es completo
                if (path.status == NavMeshPathStatus.PathComplete && path.corners.Length >= 2)
                {
                    Vector3 finalPoint = path.corners[^1];
                    Vector3 prevPoint = path.corners[^2];

                    // Dirección real del tramo final
                    Vector3 pathDirection = (finalPoint - prevPoint).normalized;

                    Vector3 rayOrigin = finalPoint + Vector3.up * height;

                    bool blocked = Physics.Raycast(rayOrigin, pathDirection, obstacleDistance, obstacleMask);

                    Debug.DrawRay(rayOrigin, pathDirection * obstacleDistance, blocked ? Color.red : Color.green, 1f);

                    if (blocked)
                    {
                        continue;
                    }
                    else
                    {
                        result = hit.position;
                        return true;
                    }
                }
            }
        }
        result = Vector3.zero;
        return false;
    }

    public bool GetReachableRandomPositionBehind(Vector3 initialPoint, NavMeshAgent agent, float radius, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 back = -transform.forward;

            float angle = Random.Range(-120f, 120f);

            Vector3 randomDir = Quaternion.Euler(0, angle, 0) * back;
            randomDir.Normalize();

            float minDistance = Mathf.Max(1f, radius * 0.7f);

            float randomDistance = Random.Range(minDistance, radius);

            Vector3 offset = randomDir * randomDistance;

            Vector3 destiny = initialPoint + offset;

            if (NavMesh.SamplePosition(destiny, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                NavMeshPath path = new();

                agent.CalculatePath(hit.position, path);

                // Solo seguimos si el path es completo
                if (path.status == NavMeshPathStatus.PathComplete && path.corners.Length >= 2)
                {
                    Vector3 finalPoint = path.corners[^1];
                    Vector3 prevPoint = path.corners[^2];

                    // Dirección real del tramo final
                    Vector3 pathDirection = (finalPoint - prevPoint).normalized;

                    Vector3 rayOrigin = finalPoint + Vector3.up * height;

                    bool blocked = Physics.Raycast(rayOrigin, pathDirection, obstacleDistance, obstacleMask);

                    Debug.DrawRay(rayOrigin, pathDirection * obstacleDistance, blocked ? Color.red : Color.green, 1f);

                    if (blocked)
                    {
                        continue;
                    }
                    else
                    {
                        result = hit.position;
                        return true;
                    }
                }
            }
        }
        result = Vector3.zero;
        return false;
    }
}
