using System;
using UnityEngine;
using UnityEngine.UI;
public abstract class Health : MonoBehaviour
{

    public float MaxHealth;
    [ReadOnly][SerializeField] protected float CurrentHealth;
    protected bool _isDead;

    protected void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public abstract void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger, GameObject Attacker = null);

    public abstract void Die();
}
