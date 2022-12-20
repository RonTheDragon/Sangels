using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : Combat
{
    List<TriggerRegistration> _meleeTriggers = new List<TriggerRegistration>();
    public float GetDamageCD;

    void Start()
    {
        StartCoroutine("WaitOneFrame");
    }
    IEnumerator WaitOneFrame()
    {
        yield return null;
        attackManager.JoinAttackerManager(this);
        foreach (TriggerRegistration meleeTrigger in _meleeTriggers)
        {
            meleeTrigger.Attackable = Attackable;
        }
    }

    public RegisteredDamaged SubmitToRegisteredObjects(Health mom) 
    {
        mom.TakeDamage(DamageAmount, Knockback, transform.position, Stagger,transform.parent.gameObject);
        if(Fire>0)mom.TakeFire(Fire);
        return new RegisteredDamaged(GetDamageCD, mom.gameObject);
    }

    public void addTrigger(TriggerRegistration tr)
    {
        _meleeTriggers.Add(tr);
    }




}
