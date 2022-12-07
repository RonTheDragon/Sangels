using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AiHealth : Health
{
    AIController aiController => GetComponent<AIController>();
    public void Dead() 
    {
        if (CurrentHealth <= 0) 
        {
            gameObject.SetActive(false);
        }
    }


    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom)
    {
        CurrentHealth -= damage;
        aiController.AddForce(pushFrom, knockback);
        Debug.Log($"Enemy took {damage} damage");
        Dead();
    }
    public override void TakeFire()
    {
        throw new NotImplementedException();
    }
    public override void TakeStun()
    {
        throw new NotImplementedException();
    }
     







}
