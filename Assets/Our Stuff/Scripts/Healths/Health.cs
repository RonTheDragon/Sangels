using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField] Image HealthBar;

    float _peviousCurrentHealth;

    Action Loop;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        Loop += OnFire;
        Loop += UpdateHealthBar;
    }

    protected void Update()
    {
        Loop?.Invoke();
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
        if (StaggerResistance >= UnityEngine.Random.Range(stagger.x, stagger.y)) return false;
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

            CurrentHealth -= _fireCurrently/ _fireMax * (_fireDamage/100);
            Die();

            if (_fireCurrently > _fireMax) _fireCurrently = _fireMax;
        }
        else if (FireParticle.isPlaying)
        {
            FireParticle.Stop();
        }
    }

    void UpdateHealthBar()
    {
        if (HealthBar != null && _peviousCurrentHealth != CurrentHealth)
        {
            float healthPercent = CurrentHealth / MaxHealth;

            // Lerp between green and yellow at 50% health
            Color color1 = Color.Lerp(Color.green, Color.yellow, (1 - healthPercent) * 2f);
            // Lerp between yellow and red at 25% health
            Color color2 = Color.Lerp(Color.yellow, Color.red, (1 - healthPercent) * 4f);
            // Use the appropriate color based on the current health
            HealthBar.color = (healthPercent > 0.5f) ? color1 : color2;
            HealthBar.fillAmount = CurrentHealth / MaxHealth;
        }
        _peviousCurrentHealth = CurrentHealth;
    }
}
