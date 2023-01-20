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

    public Action OnRevive;
    private PlayerController _playerController => (PlayerController)controller;
    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger, GameObject Attacker = null)
    {
        if (IsDead) return;

        CurrentHealth -= damage / FruitArmorEffect; //Lower Health

        float recievedStagger = CalculateReceivedStagger(Stagger); // Calculate Stagger

        EffectFromImpactType ImpactType = CalculateImpactType(recievedStagger); // Calculate Impact Type

        float recievedKnockback = CalculateKnockback(knockback, recievedStagger, ImpactType); // Calculate Knockback
        recievedKnockback *= FruitKnockEffect;
        _playerController.AddForce(-pushFrom, recievedKnockback);

        _playerController.Hurt(ImpactType, recievedStagger,StaggerResistance, Attacker);

        string AttackerName = Attacker != null ? Attacker.name : "No One";
        //  Debug.Log($"{gameObject.name} took {damage} damage and {knockback} Knockback from {AttackerName}");

        Die();
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
        _anim.SetBool("Stagger", false);//make the ai not seeing dead player
        _anim.SetBool("Fall", IsDead);
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
        _anim.SetBool("Fall", IsDead);
        OnRevive.Invoke();
        _playerController.enabled = true;
    }

}
