using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject weaponGameObject;
    public RuntimeAnimatorController runtimeAnimController;
    public WeaponComboSet weaponComboSet;
}
