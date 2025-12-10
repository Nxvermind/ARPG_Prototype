using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DashSkill : MonoBehaviour
{
    public SkinnedMeshRenderer[] playerMeshRenderers;
    [Space]
    public MeshRenderer swordMeshRenderer;
    public BoxCollider[] collidersToDeactivate;
    public bool dashSkillHitBoxActive;

    private CharacterController characterController;
    public float skillDamage;

    public Vector3 offset;
    public Vector3 halfExtends;
    public Collider[] hitResults;
    private HashSet<GameObject> hitEnemies = new ();
    public LayerMask enemyLayer;


    private void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
        hitResults = new Collider[10];
    }

    private void Update()
    {
        if (!dashSkillHitBoxActive)
        {
            return;
        }

        int hits = Physics.OverlapBoxNonAlloc(transform.position + offset, halfExtends, hitResults, transform.rotation, enemyLayer);

        // Procesa cada enemigo detectado
        for (int i = 0; i < hits; i++)
        {
            if (hitResults[i] == null) continue; // Evita procesar entradas nulas

            // Obtiene el componente Enemy del objeto golpeado
            Enemy enemy = hitResults[i].GetComponentInParent<Enemy>();

            if (enemy != null && !hitEnemies.Contains(enemy.gameObject))
            {
                // Añade el enemigo a la lista de golpeados
                hitEnemies.Add(enemy.gameObject);

                // Aplica daño al enemigo
                enemy.EnemyBlackboard.gotHit = true;
                enemy.EnemyBlackboard.gotHitByDashSkill = true;
                enemy.EnemyParameters.currentHp -= skillDamage;
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;

    //    Gizmos.DrawWireCube(transform.position + offset, halfExtends);
    //}

    public void ActivateDashSkillHitBox()
    {
        dashSkillHitBoxActive = true;
        
        foreach (var coll in collidersToDeactivate)
        {
            coll.enabled = false;
        }

    }

    public void DeactivateDashSkillHitBox()
    {
        dashSkillHitBoxActive = false;

        foreach (var coll in collidersToDeactivate)
        {
            coll.enabled = true;
        }

        for (int i = 0; i < hitResults.Length; i++)
        {
            hitResults[i] = null;
        }

        hitEnemies.Clear();

    }

    public void DeactivateMeshes()
    {
        for (int i = 0; i < playerMeshRenderers.Length; i++)
        {
            playerMeshRenderers[i].enabled = false;
            swordMeshRenderer.enabled = false;
        }

        characterController.detectCollisions = false;
    }

    public void ActivateMeshes()
    {
        for (int i = 0; i < playerMeshRenderers.Length; i++)
        {
            playerMeshRenderers[i].enabled = true;
            swordMeshRenderer.enabled = true;
        }

        characterController.detectCollisions = true;

    }
}
