using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmoSwitch : Combat
{
    PlayerCombatManager playerAttackManager => (PlayerCombatManager)attackManager;

    // Start is called before the first frame update
    void Start()
    {
        attackManager.Damagers.Add(this);
        SwitchAmmo();
        playerAttackManager.Loop += AmmoSwitching;
    }

    void AmmoSwitching()
    {
        if (playerAttackManager.AmmoTypes.Count > 1 && playerAttackManager.AvailableFruits() > 0)
        {
            if (playerAttackManager._scroll > 0)
            {
                for (int i = 0; i < playerAttackManager.AmmoTypes.Count; i++)
                {
                    playerAttackManager._currentAmmo++;
                    if (playerAttackManager._currentAmmo > playerAttackManager.AmmoTypes.Count - 1)
                    {
                        playerAttackManager._currentAmmo = 0;
                    }
                    if (playerAttackManager.AmmoTypes[playerAttackManager._currentAmmo].CurrentAmount > 0)
                    {
                        SwitchAmmo();
                        break;
                    }
                }
            }
            else if (playerAttackManager._scroll < 0)
            {
                for (int i = 0; i < playerAttackManager.AmmoTypes.Count; i++)
                {
                    playerAttackManager._currentAmmo--;
                    if (playerAttackManager._currentAmmo < 0)
                    {
                        playerAttackManager._currentAmmo = playerAttackManager.AmmoTypes.Count - 1;
                    }
                    if (playerAttackManager.AmmoTypes[playerAttackManager._currentAmmo].CurrentAmount > 0)
                    {
                        SwitchAmmo();
                        break;
                    }
                }
            }
        }
        else 
        {
            playerAttackManager.CurrentAmmo = null;
        }
    }




    void SwitchAmmo()
    {
        if (playerAttackManager.AmmoTypes.Count > 0)
        {
            playerAttackManager.CurrentAmmo = playerAttackManager.AmmoTypes[playerAttackManager._currentAmmo];
            playerAttackManager._scroll = 0;

            //Debug.Log(CurrentAmmo);
        }
        else
        {
            playerAttackManager.CurrentAmmo = null;
        }
    }
}
