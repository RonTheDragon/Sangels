using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AiHealth : Health
{
    AIController aiController => GetComponent<AIController>();

    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger, GameObject Attacker = null)
    {
        if (_isDead) return;
        CurrentHealth -= damage;
        aiController.AddForce(pushFrom, knockback);
        string AttackerName = Attacker != null ? Attacker.name : "No One";
        Debug.Log($"player took {damage} damage from {AttackerName}");

        Die();
    }
    public override void TakeFire()
    {
        throw new NotImplementedException();
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