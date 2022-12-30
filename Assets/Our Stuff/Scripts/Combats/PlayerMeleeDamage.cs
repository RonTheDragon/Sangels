using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeDamage : MeleeDamage
{
    [HideInInspector] public float FruitFireEffect = 0;
    [HideInInspector] public float FruitGlubEffect = 0;
    [HideInInspector] public float FruitKnockEffect = 1;
    [HideInInspector] public float FruitStunEffect = 1;

    public override RegisteredDamaged SubmitToRegisteredObjects(Health TargetHealth)
    {
        TargetHealth.TakeDamage(
            attackManager.SOMeleeAttack.DamageAmount,
            attackManager.SOMeleeAttack.Knockback * FruitKnockEffect,
            transform.position,
            attackManager.SOMeleeAttack.Stagger * FruitStunEffect, //Temporary here till we have real stun
            transform.parent.gameObject);

        if ((attackManager.SOMeleeAttack.Fire+ FruitFireEffect) > 0) TargetHealth.TakeFire(attackManager.SOMeleeAttack.Fire+ FruitFireEffect);

        if (FruitGlubEffect>0)  TargetHealth.TakeGlub(FruitGlubEffect); 

        return new RegisteredDamaged(GetDamageCD, TargetHealth.gameObject);
    }
}
