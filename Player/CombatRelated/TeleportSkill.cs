using System;
using System.Collections;
using UnityEngine;

public class TeleportSkill : MonoBehaviour , ISkill
{
    [SerializeField] private SkillData skillData;
    [SerializeField] private PlayerCombatContext ctx;
    [SerializeField] private float maxDistance;
    [SerializeField] private float skillHeight;
    [SerializeField] private LayerMask obstacleAndEnemyBodyLayer;

    private Player player;

    public bool IsSkillReady { get; private set; }

    public SkillData SkillData => skillData;

    public static event Action OnTeleportSkillUsed;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void Start()
    {
        IsSkillReady = true;
    }

    public void TryExecuteTeleportSkill()
    {
        if (ctx.CurrentSkillTarget == null) return;
        if (ctx.CurrentSkillTarget.position.y > .1f) return;

        Vector3 origin = ctx.transform.position + Vector3.up * skillHeight;
        Vector3 targetPos = ctx.CurrentSkillTarget.position + Vector3.up * skillHeight;

        Vector3 toTarget = (targetPos - origin).normalized;


        if (Physics.Raycast(origin, toTarget, out RaycastHit hit, maxDistance, obstacleAndEnemyBodyLayer))
        {
            if(hit.transform == ctx.CurrentSkillTarget)
            {
                ActivateTeleportSkill();
            }
        }

    }

    public void ActivateTeleportSkill()
    {
        Transform target = ctx.CurrentSkillTarget;

        OnTeleportSkillUsed?.Invoke();

        SkillsEvents.OnSkillUsedEvent(this);
        PlayerSkills.OnTeleportSkill(target);

        StartCoroutine(CooldownCoroutine());

        player.CharacterController.enabled = false;

        Vector3 backPosition = target.position - target.forward * 1.2f - target.right * 0.3f;
        backPosition.y = target.position.y;

        player.PlayerHorizontalMovement.Reset();

        ctx.PlayerTransform.SetPositionAndRotation(backPosition, target.rotation);

        player.StateMachine.ChangeState(player.PlayerStateFactory.IdleState);

        player.CharacterController.enabled = true;
    }

    IEnumerator CooldownCoroutine()
    {
        IsSkillReady = false;
        yield return new WaitForSecondsRealtime(skillData.cooldown);
        IsSkillReady = true;
    }
}
