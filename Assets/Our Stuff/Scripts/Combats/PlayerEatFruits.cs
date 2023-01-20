using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SOFruit;

public class PlayerEatFruits : Combat
{
    public Action<float, Color> OnGutChangeUI;

    private Fruit _currentFruitDigested;
    [SerializeField] private float _gutMaxCapacity = 100;
    [ReadOnly][SerializeField] private float _gutCurrert;
    [SerializeField] private float _gutDrainSpeed = 3;


    //Effected By Fruits 
    private PlayerMeleeDamage _effectMelee => GetComponent<PlayerMeleeDamage>();
    private PlayerController _effectMovement => transform.GetComponentInParent<PlayerController>();
    private PlayerHealth _effectHealth => transform.GetComponentInParent<PlayerHealth>();

    private PlayerCombatManager _playerAttackManager => (PlayerCombatManager)_attackManager;

    private Color _brown => Color.Lerp(Color.yellow, Color.Lerp(Color.red, Color.blue, 0.5f), 0.5f);

    [Header("Luber Effects")]
    [Tooltip("Jump Boost")][SerializeField] private float _luberPassive = 0.5f;
    [Tooltip("Knockback Weakness")][SerializeField] private float _luberWeakness = 2;
    [Tooltip("Knocking back Targets")][SerializeField] private float _luberMelee = 2;

    [Header("Scrumbulk Effects")]
    [Tooltip("Armor Boost")][SerializeField] private float _scrumbulkPassive = 10;
    [Tooltip("Movement Slowness")][SerializeField] private float _scrumbulkWeakness = 1;
    [Tooltip("Stun Targets")][SerializeField] private float _scrumbulkMelee = 10;

    [Header("Glub Effects")]
    [Tooltip("Knockback Resistance")][SerializeField] private float _glubPassive = 0.8f;
    [Tooltip("Jump Lower")][SerializeField] private float _glubWeakness = 0.5f;
    [Tooltip("Slow Targets")][SerializeField] private float _glubMelee = 50;

    [Header("Fepler Effects")]
    [Tooltip("Sets You On Fire")][SerializeField] private float _feplerWeakness = 50;
    [Tooltip("Burn Targets")][SerializeField] private float _feplerMelee = 50;


    // Start is called before the first frame update
    private void Start()
    {
        _attackManager.Damagers.Add(this);
        _playerAttackManager.Eat += Eat;
        _playerAttackManager.Loop += Digestion;
        ResetEffects();
    }

    private void Eat()
    {
            Digest(_playerAttackManager.CurrentAmmo.FruitType);
    }

    private void Digest(Fruit fruit)
    {
        if (_currentFruitDigested != fruit)
        {
            if (_gutCurrert > 0)
            {
                _gutCurrert = 0;
                //Burp
            }
            _currentFruitDigested=fruit;
            ResetEffects();
        }

        StartCoroutine("AddToGut");
    }

    private IEnumerator AddToGut()
    {
        yield return new WaitForSeconds(0.5f);

        // How much to add to gut
        for (int i = 1; i <= 10; i++)
        {
            if (_gutCurrert <= (_gutMaxCapacity / 10) * i)
            {
                _gutCurrert += _gutMaxCapacity / (2 + i);
                break;
            }
        }
        if (_gutCurrert > _gutMaxCapacity) { _gutCurrert = _gutMaxCapacity; }
    }

    private void Digestion()
    {
        if (_gutCurrert > 0)
        {
            _gutCurrert -= _gutDrainSpeed * Time.deltaTime;
        }
        else if (_gutCurrert < 0)
        {
            _gutCurrert = 0;
            ResetEffects();
        }
        Color gutColor=Color.white;
        switch (_currentFruitDigested)
        {
            case Fruit.Fepler: DigestingFepler(); gutColor = Color.red; break;
            case Fruit.Shbulk: DigestingScrumbulk(); gutColor = _brown; break; 
            case Fruit.Luber: DigestingLuber(); gutColor = Color.cyan; break;
            case Fruit.Glub: DigestingGlub(); gutColor = Color.black; break;
        }
        
        float fillAmount = _gutCurrert / _gutMaxCapacity;
        OnGutChangeUI?.Invoke(fillAmount, gutColor);
    }

    private void ResetEffects()
    {
        // Movement 
        _effectMovement.FruitJumpEffect = 1;
        _effectMovement.FruitSpeedEffect = 1;

        // Health
        _effectHealth.FruitKnockEffect = 1;
        _effectHealth.FruitArmorEffect = 1;
        _effectHealth.FruitFireEffect = 0;

        // Melee
        _effectMelee.FruitStunEffect = 1;
        _effectMelee.FruitKnockEffect = 1;
        _effectMelee.FruitFireEffect = 0;
        _effectMelee.FruitGlubEffect = 0;
    }

    private void DigestingLuber()
    {
        float JumpEffect = 1;
        JumpEffect += (_gutCurrert / _gutMaxCapacity)* _luberPassive;
        _effectMovement.FruitJumpEffect = JumpEffect;

        float knockEffect = 1;
        knockEffect += (_gutCurrert / _gutMaxCapacity)*_luberWeakness;
        _effectHealth.FruitKnockEffect = knockEffect;

        float MeleeKnockEffect = 1;
        MeleeKnockEffect += (_gutCurrert / _gutMaxCapacity) * _luberMelee;
        _effectMelee.FruitKnockEffect = MeleeKnockEffect;
    }

    private void DigestingScrumbulk()
    {
        float ArmorEffect = 1;
        ArmorEffect += _gutCurrert / (_gutMaxCapacity)* _scrumbulkPassive;
        _effectHealth.FruitArmorEffect = ArmorEffect;

        float SpeedEffect = 1;
        SpeedEffect -= (_gutCurrert/ (_gutMaxCapacity + _gutMaxCapacity/10))*_scrumbulkWeakness;
        _effectMovement.FruitSpeedEffect = SpeedEffect;

        float MeleeStunEffect = 1;
        MeleeStunEffect += _gutCurrert / _gutMaxCapacity * _scrumbulkMelee;
        _effectMelee.FruitStunEffect = MeleeStunEffect;
    }

    private void DigestingGlub()
    {
        float knockEffect = 1;
        knockEffect -= (_gutCurrert / _gutMaxCapacity) * _glubPassive;
        _effectHealth.FruitKnockEffect = knockEffect;

        float JumpEffect = 1;
        JumpEffect -= (_gutCurrert / _gutMaxCapacity) * _glubWeakness;
        _effectMovement.FruitJumpEffect = JumpEffect;

        float MeleeGlubEffect = 0;
        MeleeGlubEffect += (_gutCurrert / _gutMaxCapacity) * _glubMelee;
        _effectMelee.FruitGlubEffect = MeleeGlubEffect;
    }

    private void DigestingFepler()
    {
        float FireEffect = 0;
        FireEffect += _gutCurrert / _gutMaxCapacity * _feplerWeakness;
        _effectHealth.FruitFireEffect = FireEffect;

        float MeleeFireEffect = 0;
        MeleeFireEffect += _gutCurrert / _gutMaxCapacity * _feplerMelee;
        _effectMelee.FruitFireEffect = MeleeFireEffect;
    }
}
