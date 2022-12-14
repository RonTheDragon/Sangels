using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AiHealth : CharacterHealth
{
    private AIController _aiController => (AIController)controller;

    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger, GameObject Attacker = null)
    {
        if (_isDead) return;
        CurrentHealth -= damage;
        bool Staggered = TryStagger(Stagger);
        if (!Staggered) knockback *= 0.1f;
        _aiController.AddForce(pushFrom, knockback);
        string AttackerName = Attacker != null ? Attacker.name : "No One";
        Debug.Log($"{gameObject.name} took {damage} damage and {knockback} Knockback from {AttackerName}");
        _aiController.Hurt(damage/ MaxHurtAnimationDamage,Attacker, Staggered);
        Die();
    }
    public override void TakeStun()
    {
        throw new NotImplementedException();
    }

    public override void Die()
    {
        if (CurrentHealth > 0) return;
        CurrentHealth = 0;
        gameObject.SetActive(false);
        _isDead = true;
    }

}
