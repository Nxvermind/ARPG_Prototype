using System.Collections.Generic;
using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    [SerializeField] private PlayerCombatContext ctx;

    [Header("HitBox settings")]
    [SerializeField] private GameObject center;
    [SerializeField] private Vector3 halfExtents;
    [SerializeField] private LayerMask enemyLayer;
    private readonly Collider[] hitResults = new Collider[10];
    private readonly HashSet<GameObject> hitEnemies = new();
    private bool attackActive;

    private bool onceTriggered;

    private void Update()
    {
        if (!attackActive) return;

        int hits = Physics.OverlapBoxNonAlloc(center.transform.position, halfExtents, hitResults, center.transform.rotation, enemyLayer);

        for (int i = 0; i < hits; i++)
        {
            if (hitResults[i] == null) continue;

            if(hitResults[i].transform.parent.TryGetComponent<Enemy>(out var enemy))
            {
                if (!hitEnemies.Contains(enemy.gameObject))
                {
                    hitEnemies.Add(enemy.gameObject);

                    Vector3 impactPoint = hitResults[i].ClosestPoint(center.transform.position);
                    enemy.EnemyBones.GetClosestBone(impactPoint);
                    enemy.EnemyHitReciever.RecieveHit(ctx.CurrentAttackNode);
                }
            }

            if (!onceTriggered)
            {
                onceTriggered = true;
                EventBus.EnemyHitEvent();

                if (ctx.CurrentAttackNode == null) return;
                
                TimeScaler.instance.ApplyHistop(0.075f, ctx.CurrentAttackNode.hitstopDuration);
            }
        }
    }

    public void ActivateHitBox()
    {
        hitEnemies.Clear();
        onceTriggered = false;
        attackActive = true;
    }

    public void DeactivateHitBox()
    {
        attackActive = false;
        for (int i = 0; i < hitResults.Length; i++)
        {
            hitResults[i] = null;
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (center == null) return;

    //    Gizmos.color = attackActive ? Color.red : Color.green;

    //    Matrix4x4 oldMatrix = Gizmos.matrix;

    //    Gizmos.matrix = Matrix4x4.TRS(
    //        center.transform.position,
    //        center.transform.rotation,
    //        Vector3.one
    //    );

    //    Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);

    //    Gizmos.matrix = oldMatrix;
    //}
}
