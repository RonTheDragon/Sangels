using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerHealth : Health
{
    [HideInInspector] public float FruitFireEffect = 0;

    ThirdPersonMovement playerController => GetComponent<ThirdPersonMovement>();
    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger, GameObject Attacker = null)
    {
        if (_isDead) return;

        CurrentHealth -=damage;
        bool Staggered = IsStaggered(Stagger);
        if (!Staggered) knockback *= 0.4f;
        playerController.AddForce(-pushFrom, knockback);

        string AttackerName = Attacker != null ? Attacker.name : "No One";
        playerController.Hurt(damage / MaxHurtAnimationDamage,Attacker, Staggered);
        Debug.Log($"{gameObject.name} took {damage} damage and {knockback} Knockback from {AttackerName}");
    }

    new void Update()
    {
        base.Update();
        if (FruitFireEffect != _fireMin)
        {
            _fireMin = FruitFireEffect;
        }
    }

    public override void TakeStun()
    {
        throw new NotImplementedException();
    }

    public override void Die()
    {
        if (CurrentHealth > 0) return;
        CurrentHealth = 0;
        //gameObject.SetActive(false);
        Debug.Log("you fucking loser");
        _isDead = true;
        
    }


}
