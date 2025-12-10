using System.Collections;
using UnityEngine;

public class StaggerSystem : MonoBehaviour
{
    private EnemyParameters enemyParameters;

    [SerializeField] private MonoBehaviour attackProviderComponent;

    private ICurrentAttackNodeProvider attackProvider;

    public bool coroutineActive;

    private void Awake()
    {
        attackProvider = attackProviderComponent as ICurrentAttackNodeProvider;
        enemyParameters = GetComponent<EnemyParameters>();
    }
        
    public void UpdateStaggerValue()
    {
        enemyParameters.currentStaggerValue += attackProvider.CurrentAttackNode.staggerDamage;

    }

    public void BackToZeroStaggerValue(float regenAmount)
    {
        enemyParameters.currentStaggerValue = Mathf.Max(0, enemyParameters.currentStaggerValue - regenAmount);
    }

}
