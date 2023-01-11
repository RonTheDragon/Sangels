using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerHealth : CharacterHealth
{
    [HideInInspector] public float FruitFireEffect = 0;
    [HideInInspector] public float FruitKnockEffect = 1;
    [HideInInspector] public float FruitArmorEffect = 1;
    private PlayerController _playerController => (PlayerController)controller;
    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger, GameObject Attacker = null)
    {
        if (_isDead) return;
        CurrentHealth -= damage / FruitArmorEffect;
        bool Staggered = TryStagger(Stagger);
        knockback *= FruitKnockEffect;
        if (!Staggered) knockback *= 0.4f;
        _playerController.AddForce(-pushFrom, knockback);

        string AttackerName = Attacker != null ? Attacker.name : "No One";
        _playerController.Hurt(damage / MaxHurtAnimationDamage,Attacker, Staggered);
      //  Debug.Log($"{gameObject.name} took {damage} damage and {knockback} Knockback from {AttackerName}");
    }

    new private void Update()
    {
        base.Update();
        if (FruitFireEffect != _onFireSpectrum.x)
        {
            _onFireSpectrum.x = FruitFireEffect;
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

    [ContextMenu("Heal Me")]
    private void HealPlayerMaxHealth()
    {
        CurrentHealth = MaxHealth;
    }

}
