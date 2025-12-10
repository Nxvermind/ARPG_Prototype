using UnityEngine;

public class EnemyGroundDetection : MonoBehaviour
{
    public GameObject center;

    public float radius;

    public bool isGrounded;

    public LayerMask groundMask;

    private void Update()
    {
        isGrounded = Physics.CheckSphere(center.transform.position, radius, groundMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(center.transform.position, radius);
    }

}
