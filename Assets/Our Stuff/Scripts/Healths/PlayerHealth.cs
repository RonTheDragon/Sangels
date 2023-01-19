using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEditor.Animations;
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
        Die();
        if (IsDead) return;
        CurrentHealth -= damage / FruitArmorEffect;
        float Staggered = TryStagger(Stagger);
        knockback *= FruitKnockEffect;
        if (Staggered <= 2 &&Staggered >1)
            knockback *= 0.4f * Staggered;
        _playerController.AddForce(-pushFrom, knockback);

        string AttackerName = Attacker != null ? Attacker.name : "No One";
        _playerController.Hurt(Attacker, Staggered);
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



    public override void Die()
    {
        if (CurrentHealth > 0) return;
        CurrentHealth = 0;
        //gameObject.SetActive(false);
        IsDead = true;
        _playerController.
        GetComponentInChildren<Animator>().SetBool("Stagger", false);//make the ai not seeing dead player
        GetComponentInChildren<Animator>().SetBool("Fall", IsDead);
        gameObject.AddComponent<DeadPlayer>();
        _playerController.enabled = false;
    }

    [ContextMenu("Heal Player to full hp")]
    public void HealPlayerMaxHealth()
    {
        CurrentHealth = MaxHealth;
    }

    [ContextMenu("Revive Player to X hp")]
    public void RevivePlayer(int healthToRevive)
    {
        CurrentHealth = healthToRevive;
        IsDead=false;
        GetComponentInChildren<Animator>().SetBool("Fall", IsDead);
        _playerController.enabled = true;
    }

}
