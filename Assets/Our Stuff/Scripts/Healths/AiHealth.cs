using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AiHealth : CharacterHealth
{
    private AIController _aiController => (AIController)controller;

    [Header("Vitaliv")]
    [SerializeField] private string _petalBlast = "PetalBlast";
    [SerializeField] private float _petalRadius = 5;
    private float _currentVitaliv;

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
        _currentVitaliv = 0;
    }

    public void Vitaliv(float effect)
    {
        _currentVitaliv = effect;
    }

    public override void Die()
    {
        if (CurrentHealth > 0) return;
        CurrentHealth = 0;
        IsDead = true;
        
        if (_currentVitaliv > 0)
        {
            HealingBlast healingBlast = ObjectPooler.Instance.SpawnFromPool(_petalBlast, transform.position, transform.rotation).GetComponent<HealingBlast>();
            healingBlast.HealAmount = _currentVitaliv;
            healingBlast.Radius = _petalRadius;
            healingBlast.Healable = GameManager.Instance.PlayersOnly;
            healingBlast.Explode();
        }

        gameObject.SetActive(false);
    }

}
