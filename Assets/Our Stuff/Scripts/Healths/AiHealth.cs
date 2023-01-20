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
        if (IsDead) return;

        CurrentHealth -= damage; // Lower Health

        float recievedStagger = CalculateReceivedStagger(Stagger); // Calculate Stagger

        EffectFromImpactType ImpactType = CalculateImpactType(recievedStagger); // Calculate Impact Type

        float recievedKnockback = CalculateKnockback(knockback, recievedStagger, ImpactType); // Calculate Knockback
        _aiController.AddForce(pushFrom, recievedKnockback);

        _aiController.Hurt(ImpactType, recievedStagger, StaggerResistance, Attacker);

        string AttackerName = Attacker != null ? Attacker.name : "No One";
        //Debug.Log($"{gameObject.name} took {damage} damage and {knockback} Knockback from {AttackerName}");

        Die();
    }

    public override void Die()
    {
        if (CurrentHealth > 0) return;
        CurrentHealth = 0;
        gameObject.SetActive(false);
        IsDead = true;
    }

}
