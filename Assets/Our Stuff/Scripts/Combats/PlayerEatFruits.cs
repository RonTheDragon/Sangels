using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SOFruit;

public class PlayerEatFruits : Combat
{
    SOFruit.Fruit CurrentFruitDigested;
    [SerializeField] float GutMaxCapacity = 100;
    [ReadOnly][SerializeField] float GutCurrert;
    [SerializeField] float GutDrainSpeed = 3;

    [SerializeField] Image GutFill;

    //Effected By Fruits 
    PlayerMeleeDamage EffectMelee => GetComponent<PlayerMeleeDamage>();
    ThirdPersonMovement EffectMovement => transform.GetComponentInParent<ThirdPersonMovement>();
    PlayerHealth EffectHealth => transform.GetComponentInParent<PlayerHealth>();


    PlayerAmmoSwitch ammoSwitch => GetComponent<PlayerAmmoSwitch>();
    PlayerCombatManager playerAttackManager => (PlayerCombatManager)attackManager;

    Color Brown => Color.Lerp(Color.yellow, Color.Lerp(Color.red, Color.blue, 0.5f), 0.5f);

    [Header("Luber Effects")]
    [Tooltip("Jump Boost")][SerializeField] float LuberPassive = 0.5f;
    [Tooltip("Knockback Weakness")][SerializeField] float LuberWeakness = 2;
    [Tooltip("Knocking back Targets")][SerializeField] float LuberMelee = 2;

    [Header("Scrumbulk Effects")]
    [Tooltip("Armor Boost")][SerializeField] float ScrumbulkPassive = 10;
    [Tooltip("Movement Slowness")][SerializeField] float ScrumbulkWeakness = 1;
    [Tooltip("Stun Targets")][SerializeField] float ScrumbulkMelee = 10;

    [Header("Glub Effects")]
    [Tooltip("Knockback Resistance")][SerializeField] float GlubPassive = 0.8f;
    [Tooltip("Jump Lower")][SerializeField] float GlubWeakness = 0.5f;
    [Tooltip("Slow Targets")][SerializeField] float GlubMelee = 50;

    [Header("Fepler Effects")]
    [Tooltip("Sets You On Fire")][SerializeField] float FeplerWeakness = 50;
    [Tooltip("Burn Targets")][SerializeField] float FeplerMelee = 50;

    // Start is called before the first frame update
    void Start()
    {
        attackManager.Damagers.Add(this);
        playerAttackManager.Eat += Eat;
        playerAttackManager.Loop += Digestion;
        ResetEffects();
    }

    void Eat()
    {
        Digest(ammoSwitch.CurrentAmmo.fruit);
    }

    void Digest(Fruit fruit)
    {
        if (CurrentFruitDigested != fruit)
        {
            if (GutCurrert > 0)
            {
                GutCurrert = 0;
                //Burp
            }
            CurrentFruitDigested=fruit;
            ResetEffects();
        }

        StartCoroutine("AddToGut");
    }

    IEnumerator AddToGut()
    {
        yield return new WaitForSeconds(0.5f);

        // How much to add to gut
        for (int i = 1; i <= 10; i++)
        {
            if (GutCurrert <= (GutMaxCapacity / 10) * i)
            {
                GutCurrert += GutMaxCapacity / (2 + i);
                break;
            }
        }
        if (GutCurrert > GutMaxCapacity) { GutCurrert = GutMaxCapacity; }
    }

    void Digestion()
    {
        if (GutCurrert > 0)
        {
            GutCurrert -= GutDrainSpeed * Time.deltaTime;
        }
        else if (GutCurrert < 0)
        {
            GutCurrert = 0;
            ResetEffects();
        }      

        switch (CurrentFruitDigested)
        {
            case Fruit.Fepler: DigestingFepler(); GutFill.color = Color.red; break;
            case Fruit.Scrumbulk: DigestingScrumbulk(); GutFill.color = Brown; break; 
            case Fruit.Luber: DigestingLuber(); GutFill.color = Color.cyan; break;
            case Fruit.Glub: DigestingGlub(); GutFill.color = Color.black; break;
            default: break;
        }

        GutFill.fillAmount = GutCurrert / GutMaxCapacity;
    }

    void ResetEffects()
    {
        // Movement 
        EffectMovement.FruitJumpEffect = 1;
        EffectMovement.FruitSpeedEffect = 1;

        // Health
        EffectHealth.FruitKnockEffect = 1;
        EffectHealth.FruitArmorEffect = 1;
        EffectHealth.FruitFireEffect = 0;

        // Melee
        EffectMelee.FruitStunEffect = 1;
        EffectMelee.FruitKnockEffect = 1;
        EffectMelee.FruitFireEffect = 0;
        EffectMelee.FruitGlubEffect = 0;
    }

    void DigestingFepler()
    {
        float FireEffect = 0;
        FireEffect += GutCurrert / GutMaxCapacity * FeplerWeakness;
        EffectHealth.FruitFireEffect = FireEffect;

        float MeleeFireEffect = 0;
        MeleeFireEffect += GutCurrert / GutMaxCapacity * FeplerMelee;
        EffectMelee.FruitFireEffect = MeleeFireEffect;
    }

    void DigestingScrumbulk()
    {
        float ArmorEffect = 1;
        ArmorEffect += GutCurrert / (GutMaxCapacity)* ScrumbulkPassive;
        EffectHealth.FruitArmorEffect = ArmorEffect;

        float SpeedEffect = 1;
        SpeedEffect -= (GutCurrert/ (GutMaxCapacity + GutMaxCapacity/10))*ScrumbulkWeakness;
        EffectMovement.FruitSpeedEffect = SpeedEffect;

        float MeleeStunEffect = 1;
        MeleeStunEffect += GutCurrert / GutMaxCapacity * ScrumbulkMelee;
        EffectMelee.FruitStunEffect = MeleeStunEffect;
    }

    void DigestingLuber()
    {
        float JumpEffect = 1;
        JumpEffect += (GutCurrert / GutMaxCapacity)* LuberPassive;
        EffectMovement.FruitJumpEffect = JumpEffect;

        float knockEffect = 1;
        knockEffect += (GutCurrert / GutMaxCapacity)*LuberWeakness;
        EffectHealth.FruitKnockEffect = knockEffect;

        float MeleeKnockEffect = 1;
        MeleeKnockEffect += (GutCurrert / GutMaxCapacity) * LuberMelee;
        EffectMelee.FruitKnockEffect = MeleeKnockEffect;
    }

    void DigestingGlub()
    {
        float knockEffect = 1;
        knockEffect -= (GutCurrert / GutMaxCapacity) * GlubPassive;
        EffectHealth.FruitKnockEffect = knockEffect;

        float JumpEffect = 1;
        JumpEffect -= (GutCurrert / GutMaxCapacity) * GlubWeakness;
        EffectMovement.FruitJumpEffect = JumpEffect;

        float MeleeGlubEffect = 0;
        MeleeGlubEffect += (GutCurrert / GutMaxCapacity) * GlubMelee;
        EffectMelee.FruitGlubEffect = MeleeGlubEffect;
    }
}
