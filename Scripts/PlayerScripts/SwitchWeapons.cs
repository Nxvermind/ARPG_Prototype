using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapons : MonoBehaviour
{
    [Header("Light Sword Weapon")]
    [SerializeField] private GameObject lightSword;
    [SerializeField] private RuntimeAnimatorController lightSwordAnimController;
    [SerializeField] private WeaponComboSet lightSwordComboSet;
    private WeaponHitBox lightSwordWeaponHitBox;

    [Header("Great Sword Weapon")]
    [SerializeField] private GameObject greatSword;
    [SerializeField] private RuntimeAnimatorController greatSwordAnimController;
    [SerializeField] private WeaponComboSet greatSwordComboSet;
    private WeaponHitBox greatSwordWeaponHitBox;

    private ComboSystem comboSystem;

    private PlayerHitBox playerHitBox;

    private InputHandler inputHandler;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        comboSystem = GetComponentInChildren<ComboSystem>();
        inputHandler = GetComponent<InputHandler>();
        playerHitBox = GetComponentInChildren<PlayerHitBox>();

        lightSwordWeaponHitBox = lightSword.GetComponentInChildren<WeaponHitBox>();
        greatSwordWeaponHitBox = greatSword.GetComponentInChildren<WeaponHitBox>();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (inputHandler.LightSwordButton)
    //    {
    //        EquipWeapon(lightSword, lightSwordAnimController, lightSwordComboSet, lightSwordWeaponHitBox);
    //    }

    //    if (inputHandler.GreatSwordButton)
    //    {
    //        EquipWeapon(greatSword, greatSwordAnimController, greatSwordComboSet, greatSwordWeaponHitBox);
    //    }
    //}

    private void EquipWeapon(GameObject weapon, RuntimeAnimatorController controller, ComboSet comboSet, WeaponHitBox weaponHitBox)
    {
        lightSword.SetActive(false);
        greatSword.SetActive(false);

        weapon.SetActive(true);
        anim.runtimeAnimatorController = controller;
        comboSystem.SetCombo(comboSet);
        playerHitBox.currentWeaponHitbox = weaponHitBox;
    }
}
