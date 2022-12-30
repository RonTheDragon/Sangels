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
        attackManager.Damagers.Add(this);
        foreach (TriggerRegistration meleeTrigger in _meleeTriggers)
        {
            meleeTrigger.Attackable = attackManager.Attackable;
        }
    }

    public virtual RegisteredDamaged SubmitToRegisteredObjects(Health TargetHealth) 
    {
        TargetHealth.TakeDamage(
            attackManager.SOMeleeAttack.DamageAmount,
            attackManager.SOMeleeAttack.Knockback,
            transform.position,
            attackManager.SOMeleeAttack.Stagger,
            transform.parent.gameObject);

        if(attackManager.SOMeleeAttack.Fire> 0)TargetHealth.TakeFire(attackManager.SOMeleeAttack.Fire);

        return new RegisteredDamaged(GetDamageCD, TargetHealth.gameObject);
    }

    public void addTrigger(TriggerRegistration tr)
    {
        _meleeTriggers.Add(tr);
    }




}
