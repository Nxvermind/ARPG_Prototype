using System.Collections;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    [SerializeField] private GameObject center;
    [SerializeField] private LayerMask layers;
    [SerializeField] private Vector3 halfExtents;

    [Space]
    private readonly Collider[] buffer = new Collider[5];

    public bool attackActive;
    public bool hitBoxActive;
    public bool alreadyHitPlayer;

    private bool dodgeAlreadyConsidered;

    private Enemy enemy;

    public HitData HitData { get; set; }

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        //Debug.Log($"attackActive is {attackActive}");

        if (!attackActive || alreadyHitPlayer) return;

        int hits = Physics.OverlapBoxNonAlloc(center.transform.position, halfExtents, buffer, center.transform.rotation, layers);

        if(hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                Collider h = buffer[i];

                if(h == null) continue;

                if (!dodgeAlreadyConsidered && h.TryGetComponent(out DodgeSystem dodge))
                {
                    dodgeAlreadyConsidered = true;
                    //Debug.Log($"hit detected is {h.transform.name} at {Time.time}");
                    dodge.TriggerPerfectDodgeWindow();
                    dodge.GetAttacker(enemy);
                }

                if (!hitBoxActive) return;

                if (!alreadyHitPlayer && h.TryGetComponent(out PlayerHitReceiver hit))
                {
                    alreadyHitPlayer = true;
                    hit.ReceiveHit(HitData, enemy);
                }
            }
        }
    }

    public void ActivateAttack()
    {
        attackActive = true;
        dodgeAlreadyConsidered = false;
    }

    //Called in an AnimationEvent
    public void ActivateEnemyHitBox()
    {
        hitBoxActive = true;
    }

    //Called in an AnimationEvent
    public void DeactivateEnemyHitBox()
    {
        attackActive = false;
        hitBoxActive = false;
        alreadyHitPlayer = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center.transform.position, center.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);
    }
}
