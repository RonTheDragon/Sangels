using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerHealth : Health
{

    ThirdPersonMovement playerController => GetComponent<ThirdPersonMovement>();
    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom)
    {
        CurrentHealth-=damage;
        playerController.AddForce(pushFrom, knockback);
            death();
        Debug.Log($"player took {damage} damage");
    }
    public override void TakeFire()
    {
        throw new NotImplementedException();
    }
    public override void TakeStun()
    {
        throw new NotImplementedException();
    }

    void death() 
    {
        // gameObject.SetActive(false);
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
        }
        Debug.Log("you fucking loser");
        
    }


}
