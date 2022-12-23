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
    MeleeDamage EffectMelee => GetComponent<MeleeDamage>();
    ThirdPersonMovement EffectMovement => transform.GetComponentInParent<ThirdPersonMovement>();
    PlayerHealth EffectHealth => transform.GetComponentInParent<PlayerHealth>();


    PlayerAmmoSwitch ammoSwitch => GetComponent<PlayerAmmoSwitch>();
    PlayerCombatManager playerAttackManager => (PlayerCombatManager)attackManager;

    Color Brown => Color.Lerp(Color.yellow, Color.Lerp(Color.red, Color.blue, 0.5f), 0.5f);

    // Start is called before the first frame update
    void Start()
    {
        attackManager.JoinAttackerManager(this);
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
            case Fruit.Albert: DigestingAlbert(); GutFill.color = Brown; break; 
            case Fruit.Luber: DigestingLuber(); GutFill.color = Color.cyan; break;
            default: break;
        }

        GutFill.fillAmount = GutCurrert / GutMaxCapacity;
    }

    void ResetEffects()
    {
        EffectMovement.FruitSpeedEffect = 1;
        EffectMovement.FruitJumpEffect = 1;
        EffectHealth.FruitFireEffect = 0;
        EffectHealth.FruitArmorEffect = 1;
        EffectHealth.FruitKnockEffect = 1;
    }




    void DigestingFepler()
    {
        float FireEffect = 0;
        FireEffect += GutCurrert / GutMaxCapacity * 50;
        EffectHealth.FruitFireEffect = FireEffect;
    }

    void DigestingAlbert()
    {
        float SpeedEffect = 1;
        SpeedEffect -= GutCurrert/ (GutMaxCapacity + GutMaxCapacity/10);
        EffectMovement.FruitSpeedEffect = SpeedEffect;

        float ArmorEffect = 1;
        ArmorEffect += GutCurrert / (GutMaxCapacity)*10;
        EffectHealth.FruitArmorEffect = ArmorEffect;
    }

    void DigestingLuber()
    {
        float JumpEffect = 1;
        JumpEffect += (GutCurrert / GutMaxCapacity)*0.5f;
        EffectMovement.FruitJumpEffect = JumpEffect;

        float knockEffect = 1;
        knockEffect += (GutCurrert / GutMaxCapacity)*2;
        EffectHealth.FruitKnockEffect = knockEffect;

    }


 

}
