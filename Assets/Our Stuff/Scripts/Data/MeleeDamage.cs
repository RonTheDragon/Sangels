using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : Damage
{
    [ReadOnly][SerializeField]List<TriggerRegistration> _meleeTriggers;
    public float GetDamageCD;
    
    Animator anim => GetComponent<Animator>();

    void Start()
    {
        foreach (TriggerRegistration meleeTrigger in _meleeTriggers)
        {
            meleeTrigger.Attackable = Attackable;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            anim.SetTrigger("PunchCombo");

        }
    }




    public RegisteredDamaged SubmitToRegisteredObjects(Health mom) 
    {
        mom.TakeDamage(DamageAmount, Knockback, transform.position);
        return new RegisteredDamaged(GetDamageCD, mom.gameObject);
    }



}
