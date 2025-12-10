using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    public WeaponHitBox currentWeaponHitbox;

    public void ActivateWeaponHitBox()
    {
        currentWeaponHitbox.ActivateHitBox();
    }

    public void DeactivateWeaponHitBox()
    {
        currentWeaponHitbox.DeactivateHitBox();
    }
}
