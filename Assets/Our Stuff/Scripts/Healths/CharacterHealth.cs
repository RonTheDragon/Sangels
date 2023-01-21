using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterHealth : Health
{
    //data

    [Header("Stagger")]
    [SerializeField] protected float StaggerResistance;
    [ReadOnly] public bool IsStaggered;
    [ReadOnly] public bool IsStunned;
    [ReadOnly] public bool IsGettingUp;

    [Tooltip("The closer the damage amount to this number, the more painful the hurt animation looks")]
    [SerializeField] protected float MaxHurtAnimationDamage = 15;

    [Header("Fire")]
    [ReadOnly][SerializeField] protected float _fireCurrently;
    [Tooltip("X = Minimum Fire (the lowest you can get, if 0 then not on fire)\nY = Maximum Fire (how much on fire you can become)")]
    protected Vector2 _onFireSpectrum = new Vector2(0,100);
    [SerializeField] protected float _fireDamage = 10;
    [SerializeField] protected float _fireExtinguishing = 25;
    [SerializeField] protected ParticleSystem FireParticle;

    protected Animator _anim => GetComponentInChildren<Animator>();
    protected Controller controller => GetComponent<Controller>();

    private float _peviousCurrentHealth;
    protected Action _loop;
    public Action<float, Color> OnHealthChangeUI;

    private float _healingAmount, _healingTime;


    public enum EffectFromImpactType
    {
        Hurt,
        Stagger,
        Stun
    }

    protected new void Start()
    {
        base.Start();
        _loop += OnFire;
        _loop += Healing;
        _loop += UpdateHealthBar;
    }

    protected void Update()
    {
        _loop?.Invoke();
    }

    #region Receive Special Effects
    public virtual void TakeFire(float Fire)
    {
        _fireCurrently += Fire;
    }

    public virtual void TakeGlub(float glub)
    {
        controller.AddGlub(glub);
    }
    #endregion

    protected float CalculateReceivedStagger(Vector2 stagger)
    {
        float staggerValue = UnityEngine.Random.Range(stagger.x, stagger.y);

        return staggerValue;
    }

    protected EffectFromImpactType CalculateImpactType(float RecievedStagger)
    {
        EffectFromImpactType ImpactType = EffectFromImpactType.Hurt;

        if (RecievedStagger < StaggerResistance * 2 && !IsStaggered && !IsStunned && !IsGettingUp) // if can Stagger
        {
            ImpactType = EffectFromImpactType.Stagger;
            IsStaggered= true;
        }

        else if (RecievedStagger >= StaggerResistance * 2 &&!IsStunned &&!IsGettingUp) // if can Stun
        {
            ImpactType = EffectFromImpactType.Stun;
            IsStunned= true;
        }

        return ImpactType;
    }

    protected float CalculateKnockback(float Knockback,float Stagger,EffectFromImpactType ImpactType)
    {
        float ExtraKnockback;
        switch (ImpactType)
        {
            case EffectFromImpactType.Stagger:
                ExtraKnockback = (Stagger / StaggerResistance) -1;
                Knockback *= Mathf.Lerp(1.4f,1.8f,ExtraKnockback);
                return Knockback;

            case EffectFromImpactType.Stun:
                ExtraKnockback = Stagger - (StaggerResistance * 2);
                ExtraKnockback = ExtraKnockback / 100;
                Knockback *= 1.8f + ExtraKnockback;
                return Knockback;

            default: 
                return Knockback;
        }
    }

    public void Heal(float amount, float time)
    {
        _healingTime = time;
        _healingAmount = amount;
    }

    public void HealInstantly(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth>MaxHealth) { CurrentHealth = MaxHealth; }
    }

    protected virtual void Healing()
    {
        if (_healingTime > 0)
        {
            CurrentHealth += _healingAmount * Time.deltaTime;
            _healingTime -= Time.deltaTime;
        }
    }

    protected virtual void OnFire()
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


    protected void UpdateHealthBar()
    {
        if(OnHealthChangeUI!=null)
        {
            if (_peviousCurrentHealth != CurrentHealth)
            {
                float healthPercent = CurrentHealth / MaxHealth;

                // Lerp between green and yellow at 50% health
                Color color1 = Color.Lerp(Color.green, Color.yellow, (1 - healthPercent) * 2f);
                // Lerp between yellow and red at 25% health
                Color color2 = Color.Lerp(Color.yellow, Color.red, (0.5f - healthPercent) * 2f);
                // Use the appropriate color based on the current health
                Color color3= (healthPercent > 0.5f) ? color1 : color2;
                float fillAmount = CurrentHealth / MaxHealth;
                OnHealthChangeUI?.Invoke(fillAmount, color3);
            }
            _peviousCurrentHealth = CurrentHealth;

        }
    }


}
