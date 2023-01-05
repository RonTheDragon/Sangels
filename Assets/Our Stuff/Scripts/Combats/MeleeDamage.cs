using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : Combat
{
    public float GetDamageCD;
    private List<TriggerRegistration> _meleeTriggers = new List<TriggerRegistration>();

    private void Start()
    {
        StartCoroutine("WaitOneFrame");
    }
    private IEnumerator WaitOneFrame()
    {
        yield return null;
        _attackManager.Damagers.Add(this);
        foreach (TriggerRegistration meleeTrigger in _meleeTriggers)
        {
            meleeTrigger.Attackable = _attackManager.Attackable;
        }
    }

    public virtual RegisteredDamaged SubmitToRegisteredObjects(Health TargetHealth) 
    {
        TargetHealth.TakeDamage(
            _attackManager.SOMeleeAttack.DamageAmount,
            _attackManager.SOMeleeAttack.Knockback,
            transform.position,
            _attackManager.SOMeleeAttack.Stagger,
            transform.parent.gameObject);
        if (TargetHealth is CharacterHealth)
        {
            CharacterHealth characterHealth = (CharacterHealth) TargetHealth;
            if (_attackManager.SOMeleeAttack.Fire > 0) characterHealth.TakeFire(_attackManager.SOMeleeAttack.Fire);
        }

        return new RegisteredDamaged(GetDamageCD, TargetHealth.gameObject);
    }

    public void AddTrigger(TriggerRegistration tr)
    {
        _meleeTriggers.Add(tr);
    }




}
