using System.Collections.Generic;
using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    [Header("HitBox settings")]
    public GameObject center;
    public Vector3 halfExtents; // Tamaño del box collider
    public LayerMask enemyLayer;
    private readonly Collider[] hitResults = new Collider[10]; // Array para almacenar colisiones
    private readonly HashSet<GameObject> hitEnemies = new(); // Lista de enemigos golpeados
    private bool attackActive;

    private bool onceTriggered;

    private void Update()
    {
        if (!attackActive)
        {
            return; // No procesar si el ataque no está activo
        }

        // Detecta colisiones con el box
        int hits = Physics.OverlapBoxNonAlloc(center.transform.position, halfExtents, hitResults, center.transform.rotation, enemyLayer);

        // Procesa cada enemigo detectado
        for (int i = 0; i < hits; i++)
        {
            if (hitResults[i] == null) continue; // Evita procesar entradas nulas

            // Obtiene el componente Enemy del objeto golpeado
            Enemy enemy = hitResults[i].GetComponentInParent<Enemy>();
            EliteEnemy eliteEnemy = enemy as EliteEnemy;

            if (enemy != null && !hitEnemies.Contains(enemy.gameObject))
            {
                // Añade el enemigo a la lista de golpeados
                hitEnemies.Add(enemy.gameObject);

                if(enemy.enemyType == EnemyType.Elite)
                {
                    eliteEnemy.hitCounter++;
                }

                // Aplica daño al enemigo
                enemy.EnemyBlackboard.gotHit = true;
                EventBus.EnemyGotHitEvent(enemy);

                if (!onceTriggered)
                {
                    EventBus.EnemyHitEvent();
                    onceTriggered = true;
                }
                //Debug.Log($"Enemy {enemy.name} detected and hit!");
            }
        }
    }

    public void ActivateHitBox()
    {
        hitEnemies.Clear(); // Limpia la lista al inicio del ataque
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
}
