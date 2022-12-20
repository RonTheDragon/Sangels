using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
//using UnityEditor.Experimental.GraphView;
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

    [Header("Fire")]
    [SerializeField] protected float _fireCurrently;
    protected float _fireMin = 0;
    protected float _fireMax =100;
    [SerializeField] protected float _fireDamage = 10;
    [SerializeField] protected float _fireExtinguishing = 25;
    [SerializeField] protected ParticleSystem FireParticle;

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    protected void Update()
    {
        OnFire();
    }

    public abstract void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger,GameObject Attacker = null);

    public virtual void TakeFire(float Fire)
    {
        _fireCurrently += Fire;
    }

    public abstract void TakeStun();

    public abstract void Die();

    protected bool IsStaggered(Vector2 stagger)
    {
        if (StaggerResistance >= Random.Range(stagger.x, stagger.y)) return false;
        return true;
    }

    void OnFire()
    {
        if (_fireCurrently < _fireMin) { _fireCurrently = _fireMin; }
        else { _fireCurrently -= _fireExtinguishing*Time.deltaTime; }

        if (_fireCurrently > 0)
        {
            if (!FireParticle.isPlaying) FireParticle.Play();
            var e = FireParticle.emission;
            e.rateOverTimeMultiplier = _fireCurrently/10;

            CurrentHealth -= _fireCurrently/ _fireMax * _fireDamage;

            if (_fireCurrently > _fireMax) _fireCurrently = _fireMax;
        }
        else if (FireParticle.isPlaying)
        {
            FireParticle.Stop();
        }
    }
}
