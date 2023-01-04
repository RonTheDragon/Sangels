using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterHealth : Health
{
    //data

    [SerializeField] protected float StaggerResistance;

    [Tooltip("The closer the damage amount to this number, the more painful the hurt animation looks")]
    [SerializeField] protected float MaxHurtAnimationDamage = 15;

    [Header("Fire")]
    [ReadOnly][SerializeField] protected float _fireCurrently;
    protected float _fireMin = 0;
    protected float _fireMax = 100;
    [SerializeField] protected float _fireDamage = 10;
    [SerializeField] protected float _fireExtinguishing = 25;
    [SerializeField] protected ParticleSystem FireParticle;

    protected Controllers controller => GetComponent<Controllers>();

    [SerializeField] Image HealthBar;

    float _peviousCurrentHealth;
    Action Loop;

    public bool IsStagged;
    new void Start()
    {
        base.Start();
        Loop += OnFire;
        Loop += UpdateHealthBar;
        controller.OnStagger += Stagged;
    }

    protected void Update()
    {
        Loop?.Invoke();
    }

    public virtual void TakeFire(float Fire)
    {
        _fireCurrently += Fire;
    }

    public virtual void TakeGlub(float glub)
    {
        controller.AddGlub(glub);
    }

    protected bool IsStaggered(Vector2 stagger)
    {
        if (StaggerResistance >= UnityEngine.Random.Range(stagger.x, stagger.y)) return false;
        return true;
    }

    void Stagged()
    {
        IsStagged = true;
    }

    void OnFire()
    {
        if (_fireCurrently < _fireMin) { _fireCurrently = _fireMin; }
        else { _fireCurrently -= _fireExtinguishing * Time.deltaTime; }

        if (_fireCurrently > 0)
        {
            if (!FireParticle.isPlaying) FireParticle.Play();
            var e = FireParticle.emission;
            e.rateOverTimeMultiplier = _fireCurrently / 10;

            CurrentHealth -= _fireCurrently / _fireMax * (_fireDamage / 100);
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
            Color color2 = Color.Lerp(Color.yellow, Color.red, (0.5f - healthPercent) * 2f);
            // Use the appropriate color based on the current health
            HealthBar.color = (healthPercent > 0.5f) ? color1 : color2;
            HealthBar.fillAmount = CurrentHealth / MaxHealth;
        }
        _peviousCurrentHealth = CurrentHealth;
    }

    public abstract void TakeStun();
}
