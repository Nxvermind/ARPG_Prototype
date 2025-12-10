using System.Collections;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    [SerializeField] private GameObject center;
    [SerializeField] private LayerMask layers;
    [SerializeField] private Vector3 halfExtents;

    [Space]
    private readonly Collider[] buffer = new Collider[3];

    public bool attackActive;
    public bool alreadyHit;

    private Enemy enemy;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (!attackActive || alreadyHit)
            return;

        int hits = Physics.OverlapBoxNonAlloc(center.transform.position, halfExtents, buffer, center.transform.rotation, layers);

        if(hits > 0)
        {
            alreadyHit = true;

            var hitCollider = buffer[0];

            if (hitCollider.TryGetComponent<IHitable>(out var hit))
            {
                hit.OnHit(enemy);
            }
            else if (hitCollider.GetComponentInParent<IHitable>() is IHitable parentHit)
            {
                parentHit.OnHit(enemy);
            }
        }
    }

    //Called in an AnimationEvent
    public void ActivateEnemyHitBox()
    {
        attackActive = true;
        alreadyHit = false; 
    }

    //Called in an AnimationEvent
    public void DeactivateEnemyHitBox()
    {
        attackActive = false;
        alreadyHit = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center.transform.position, center.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);
    }
}
