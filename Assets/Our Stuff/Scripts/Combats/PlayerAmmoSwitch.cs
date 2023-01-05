using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmoSwitch : Combat
{
    private PlayerCombatManager _playerAttackManager => (PlayerCombatManager)_attackManager;

    // Start is called before the first frame update
    private void Start()
    {
        _attackManager.Damagers.Add(this);
        SwitchAmmo();
        _playerAttackManager.Loop += AmmoSwitching;
    }

    private void AmmoSwitching()
    {
        if (_playerAttackManager.AmmoTypes.Count > 1 && _playerAttackManager.AvailableFruits() > 0)
        {
            if (_playerAttackManager.UseScroll > 0)
            {
                for (int i = 0; i < _playerAttackManager.AmmoTypes.Count; i++)
                {
                    _playerAttackManager.CurrentAmmoSlot++;
                    if (_playerAttackManager.CurrentAmmoSlot > _playerAttackManager.AmmoTypes.Count - 1)
                    {
                        _playerAttackManager.CurrentAmmoSlot = 0;
                    }
                    if (_playerAttackManager.AmmoTypes[_playerAttackManager.CurrentAmmoSlot].CurrentAmount > 0)
                    {
                        SwitchAmmo();
                        break;
                    }
                }
            }
            else if (_playerAttackManager.UseScroll < 0)
            {
                for (int i = 0; i < _playerAttackManager.AmmoTypes.Count; i++)
                {
                    _playerAttackManager.CurrentAmmoSlot--;
                    if (_playerAttackManager.CurrentAmmoSlot < 0)
                    {
                        _playerAttackManager.CurrentAmmoSlot = _playerAttackManager.AmmoTypes.Count - 1;
                    }
                    if (_playerAttackManager.AmmoTypes[_playerAttackManager.CurrentAmmoSlot].CurrentAmount > 0)
                    {
                        SwitchAmmo();
                        break;
                    }
                }
            }
        }
        else 
        {
            _playerAttackManager.CurrentAmmo = null;
        }
    }




    private void SwitchAmmo()
    {
        if (_playerAttackManager.AmmoTypes.Count > 0)
        {
            _playerAttackManager.CurrentAmmo = _playerAttackManager.AmmoTypes[_playerAttackManager.CurrentAmmoSlot];
            _playerAttackManager.UseScroll = 0;

            //Debug.Log(CurrentAmmo);
        }
        else
        {
            _playerAttackManager.CurrentAmmo = null;
        }
    }
}
