using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerHealth : Health
{

    ThirdPersonMovement playerController => GetComponent<ThirdPersonMovement>();
    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger, GameObject Attacker = null)
    {
        if (_isDead) return;

        CurrentHealth -=damage;
        playerController.AddForce(-pushFrom, knockback);

        string AttackerName = Attacker != null ? Attacker.name : "No One";
        
        Debug.Log($"player took {damage} damage from {AttackerName}");
    }
    public override void TakeFire()
    {
        throw new NotImplementedException();
    }
    public override void TakeStun()
    {
        throw new NotImplementedException();
    }

    public override void Die()
    {
        if (CurrentHealth > 0) return;
        CurrentHealth = 0;
        //gameObject.SetActive(false);
        Debug.Log("you fucking loser");
        _isDead = true;
        
    }


}
