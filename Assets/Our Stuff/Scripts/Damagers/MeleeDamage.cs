using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : Damage
{
    List<TriggerRegistration> _meleeTriggers = new List<TriggerRegistration>();
    public float GetDamageCD;

    void Start()
    {
        attackManager.Damagers.Add(this);
        Attackable = attackManager.Attackable;

        foreach (TriggerRegistration meleeTrigger in _meleeTriggers)
        {
            meleeTrigger.Attackable = Attackable;
        }
    }

    public RegisteredDamaged SubmitToRegisteredObjects(Health mom) 
    {
        mom.TakeDamage(DamageAmount, Knockback, transform.position, Stagger,transform.parent.gameObject);
        return new RegisteredDamaged(GetDamageCD, mom.gameObject);
    }

    public void addTrigger(TriggerRegistration tr)
    {
        _meleeTriggers.Add(tr);
    }




}
