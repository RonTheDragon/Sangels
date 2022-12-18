using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmoSwitch : Combat
{
    [Header("Ammo Switching")]
    [ReadOnly]public SOFruit CurrentAmmo;
    [SerializeField] List<SOFruit> AmmoTypes;
    PlayerCombatManager playerAttackManager => (PlayerCombatManager)attackManager;
    int _currentAmmo;

    // Start is called before the first frame update
    void Start()
    {
        attackManager.JoinAttackerManager(this);
        SwitchAmmo();
        playerAttackManager.Loop += AmmoSwitching;
    }

    void AmmoSwitching()
    {
        if (AmmoTypes.Count > 1)
        {
            if (playerAttackManager._scroll > 0)
            {
                _currentAmmo++;
                if (_currentAmmo > AmmoTypes.Count - 1)
                {
                    _currentAmmo = 0;
                }
                SwitchAmmo();
            }
            else if (playerAttackManager._scroll < 0)
            {
                _currentAmmo--;
                if (_currentAmmo < 0)
                {
                    _currentAmmo = AmmoTypes.Count - 1;
                }
                SwitchAmmo();
            }
        }
    }




    void SwitchAmmo()
    {
        if (AmmoTypes.Count > 0)
        {
            CurrentAmmo = AmmoTypes[_currentAmmo];
            playerAttackManager._scroll = 0;

            //Debug.Log(CurrentAmmo);
        }
        else
        {
            CurrentAmmo = null;
        }
    }
}
