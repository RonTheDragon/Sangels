using System;
using UnityEngine;

public class PlayerHealth : CharacterHealth
{
    [Tooltip("Health Regained Per Second")]
    [SerializeField] private float _naturalHealing = 3;
    [Tooltip("The Amount of Time That the player needs not be hurt")]
    [SerializeField] private float _safeTimeForNaturalHealing = 5;
    [Tooltip("from 0% to 100%\nHealth Heal Itself by Natural Healing Only Up To This Point")]
    [SerializeField] private float _naturalHealingLimit = 50;
    private float _naturalHealingCD;


    [HideInInspector] public float FruitFireEffect = 0;
    [HideInInspector] public float FruitKnockEffect = 1;
    [HideInInspector] public float FruitArmorEffect = 1;
    [HideInInspector] public float FruitHealEffect = 0;

    public Action OnHurt;
    public Action OnRevive;
    public Action OnDeath;
    private PlayerController _playerController => (PlayerController)controller;
    private Interaction _interaction => GetComponent<Interaction>();

    private DeadPlayer _deadPlayer;

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
        OnHurt?.Invoke();

        string AttackerName = Attacker != null ? Attacker.name : "No One";
        //  Debug.Log($"{gameObject.name} took {damage} damage and {knockback} Knockback from {AttackerName}");

        _naturalHealingCD = _safeTimeForNaturalHealing; // cant natural heal because of damage

        Die();
    }

    new protected void Start()
    {
        base.Start();
        _loop += FireFruitEffect;
        _loop += NaturalHealing;
        _loop += FruitHealing;
    }

    new protected void Update()
    {
        base.Update();
    }



    public override void Die()
    {
        if (CurrentHealth > 0 || IsDead) return;
        OnDeath?.Invoke();
        CurrentHealth = 0;
        //gameObject.SetActive(false);
        IsDead = true;
        _anim.SetBool("Stagger", false);//make the ai not seeing dead player
        _anim.SetBool("Fall", IsDead);
        _deadPlayer = gameObject.AddComponent<DeadPlayer>();
        _playerController.enabled = false;
        _interaction.enabled = false;
    }

    #region Fire
    protected void FireFruitEffect()
    {
        if (FruitFireEffect != _onFireSpectrum.x)
        {
            _onFireSpectrum.x = FruitFireEffect;
        }
    }
    protected override void OnFire()
    {
        base.OnFire();
        if (_fireCurrently > 0) { _naturalHealingCD = _safeTimeForNaturalHealing; }
    }
    #endregion

    #region Healing & Reviving

    protected void NaturalHealing()
    {
        if (IsDead) return;
        if (!_playerController.CheckIfCanMove()) // cant natural heal because under bad conditions
        {
            _naturalHealingCD = _safeTimeForNaturalHealing; 
        }

        if (_naturalHealingCD < 0)
        {
            _naturalHealingCD = 0;
        }
        else if (_naturalHealingCD == 0)
        {
            if (CurrentHealth < MaxHealth*_naturalHealingLimit*0.01f)
            {
                CurrentHealth += _naturalHealing * Time.deltaTime;
            }
        }
        else
        {
            _naturalHealingCD -=Time.deltaTime;
        }

        if (CurrentHealth > MaxHealth) { CurrentHealth = MaxHealth; }
    }

    protected void FruitHealing()
    {
        if (IsDead) return;
        if (FruitHealEffect > 0)
        {
            CurrentHealth += FruitHealEffect * Time.deltaTime;
        }
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
        Destroy(_deadPlayer);
        OnRevive.Invoke();
        _playerController.enabled = true;
        _interaction.enabled = true;
    }
    #endregion
}
