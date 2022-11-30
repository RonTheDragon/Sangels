using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : Damage
{
    List<TriggerRegistration> _meleeTriggers;
    public float GetDamageCD;


    // Start is called before the first frame update
    void Start()
    {
        foreach (TriggerRegistration meleeTrigger in _meleeTriggers)
        {
            meleeTrigger.Attackable = Attackable;
        }
    }

    public RegisteredDamaged SubmitToRegisteredObjects(Health mom) 
    {
        mom.TakeDamage(DamageAmount, Knockback, transform.position);
        return new RegisteredDamaged(GetDamageCD, mom.gameObject);
    }



}
