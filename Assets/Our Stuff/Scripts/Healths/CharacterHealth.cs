using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterHealth : Health
{
    //data

    [Header("Stagger")]
    [SerializeField] protected float StaggerResistance;
    [ReadOnly] public bool IsStaggered;

    [Tooltip("The closer the damage amount to this number, the more painful the hurt animation looks")]
    [SerializeField] protected float MaxHurtAnimationDamage = 15;

    [Header("Fire")]
    [ReadOnly][SerializeField] protected float _fireCurrently;
    [Tooltip("X = Minium Fire (the lowest you can get, if 0 then not on fire)\nY = Maximum Fire (how much on fire you can become)")]
    protected Vector2 _onFireSpectrum = new Vector2(0,100);
    [SerializeField] protected float _fireDamage = 10;
    [SerializeField] protected float _fireExtinguishing = 25;
    [SerializeField] protected ParticleSystem FireParticle;


    protected Controller controller => GetComponent<Controller>();

    [SerializeField] private Image _healthBar;
    private float _peviousCurrentHealth;
    private Action _loop;

    new void Start()
    {
        base.Start();
        _loop += OnFire;
        _loop += UpdateHealthBar;
        controller.OnStagger += () => IsStaggered=true;
    }

    protected void Update()
    {
        _loop?.Invoke();
    }

    public virtual void TakeFire(float Fire)
    {
        _fireCurrently += Fire;
    }

    public virtual void TakeGlub(float glub)
    {
        controller.AddGlub(glub);
    }

    protected bool TryStagger(Vector2 stagger)
    {
        if (StaggerResistance >= UnityEngine.Random.Range(stagger.x, stagger.y)) return false;
        return true;
    }

    void OnFire()
    {
        if (_fireCurrently < _onFireSpectrum.x) { _fireCurrently = _onFireSpectrum.x; }
        else { _fireCurrently -= _fireExtinguishing * Time.deltaTime; }

        if (_fireCurrently > 0)
        {
            if (!FireParticle.isPlaying) FireParticle.Play();
            var e = FireParticle.emission;
            e.rateOverTimeMultiplier = _fireCurrently / 10;

            CurrentHealth -= _fireCurrently / _onFireSpectrum.y * (_fireDamage / 100);
            Die();

            if (_fireCurrently > _onFireSpectrum.y) _fireCurrently = _onFireSpectrum.y;
        }
        else if (FireParticle.isPlaying)
        {
            FireParticle.Stop();
        }
    }

    void UpdateHealthBar()
    {
        if (_healthBar != null && _peviousCurrentHealth != CurrentHealth)
        {
            float healthPercent = CurrentHealth / MaxHealth;

            // Lerp between green and yellow at 50% health
            Color color1 = Color.Lerp(Color.green, Color.yellow, (1 - healthPercent) * 2f);
            // Lerp between yellow and red at 25% health
            Color color2 = Color.Lerp(Color.yellow, Color.red, (0.5f - healthPercent) * 2f);
            // Use the appropriate color based on the current health
            _healthBar.color = (healthPercent > 0.5f) ? color1 : color2;
            _healthBar.fillAmount = CurrentHealth / MaxHealth;
        }
        _peviousCurrentHealth = CurrentHealth;
    }

    public abstract void TakeStun();
}
