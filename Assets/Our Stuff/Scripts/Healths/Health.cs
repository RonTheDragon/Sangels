using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public abstract class Health : MonoBehaviour
{
    //data
    public float MaxHealth;
    [ReadOnly][SerializeField] protected float CurrentHealth;

    protected bool _isDead;


    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public abstract void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger,GameObject Attacker = null);

    public abstract void TakeFire();

    public abstract void TakeStun();

    public abstract void Die();

}
