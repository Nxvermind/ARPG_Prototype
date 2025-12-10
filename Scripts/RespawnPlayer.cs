using System;
using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{
    public Player player;

    public Transform respawnPoint;

    public UltimateSkill_UI ultimateSkill_UI;

    public SkillUIManager skillUIManager;

    public CanvasDeath canvasDeath;

    public static event Action OnPlayerRespawn;

    public void Respawn()
    {
        player.playerRespawned = true;
        player.transform.position = respawnPoint.position;
        player.Parameters.currentHp = player.Parameters.maxHP;
        player.Parameters.currentStamina = player.Parameters.maxStamina;
        ultimateSkill_UI.BerserkBarZero();

        skillUIManager.ResetSkills();
        canvasDeath.DeactivatePanel();

        OnPlayerRespawn?.Invoke();
    }
}
