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

    [SerializeField] protected float StaggerResistance;

    [Tooltip("The closer the damage amount to this number, the more painful the hurt animation looks")]
    [SerializeField] protected float MaxHurtAnimationDamage = 15;
    protected bool _isDead;


    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public abstract void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger,GameObject Attacker = null);

    public abstract void TakeFire();

    public abstract void TakeStun();

    public abstract void Die();

    protected bool IsStaggered(Vector2 stagger)
    {
        if (StaggerResistance >= Random.Range(stagger.x, stagger.y)) return false;
        return true;
    }

}
