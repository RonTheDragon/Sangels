using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom)
    {
        CurrentHealth-=damage;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            death();
        }
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

        Debug.Log("you fucking loser");
        
    }


}
