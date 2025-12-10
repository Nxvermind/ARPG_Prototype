using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/AttackSettings")]
public class EnemyAttackSettings : ScriptableObject
{
    [System.Serializable]
    public struct ComboAttackData
    {
        [Tooltip("this is optional")]
        public string name;
        [Tooltip("Probability to activate, percentage number")]
        public float probabilityToActivate;
        public EnemyAttackData[] comboAttack;
    }

    public EnemyAttackData[] basicLightAttack;

    public ComboAttackData[] comboAttackDatas;

    public EnemyAttackData GetRandomBasicAttack()
    {
        int index = Random.Range(0, basicLightAttack.Length);

        return basicLightAttack[index];
    }
}
