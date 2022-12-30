using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatManager : CombatManager
{
    [Header("Ammo Switching")]
    [ReadOnly] public SOFruit CurrentAmmo;
    public List<SOFruit> AmmoTypes;
    [ReadOnly] public int _currentAmmo;
    public bool RefillAmmoOnStart;

    [Header("Refrences")]
    public Camera Cam;
    public CinemachineFreeLook Cinemachine;
    ThirdPersonMovement TPM => GetComponentInParent<ThirdPersonMovement>();

    [HideInInspector]
    public bool _shoot;
    [HideInInspector]
    public float _scroll;

    bool _shootLastFrame;
    bool _holdingFire;
    bool _eat;

    public Action Shoot;
    public Action OnStopHoldShoot;
    public Action Eat;


    private void Start()
    {
        if (RefillAmmoOnStart)
        {
            RefillAllAmmo();
        }

        Attackable = GM.PlayersCanAttack;
        Loop += Melee;
        Loop += Shooting;
        Loop += Eating;
        TPM.OnStagger += Staggered;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        Loop?.Invoke();
    }



    //Inputs
    public void OnShoot(InputAction.CallbackContext context)
    {
        _shoot = context.action.triggered;
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        _melee = context.action.triggered;
    }

    public void OnEat(InputAction.CallbackContext context)
    {
        _eat = context.action.triggered;
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        _scroll = context.action.ReadValue<float>();
    }

    protected override void AttackEnded()
    {
        TPM.SetSpeed(TPM.NormalSpeed);
    }

    void Melee()
    {
        if (_melee && UsingAttackTimeLeft==0)
        {
            anim.SetTrigger(SOMeleeAttack.AnimationName);
            TPM.SetSpeed(SOMeleeAttack.speedWhileUsing);
            UsingAttackTimeLeft = SOMeleeAttack.UsingTime;
        }
    }

    void Eating()
    {
        if (CurrentAmmo == null) return;
        if (_eat && UsingAttackTimeLeft == 0 && ConsumeAmmo())
        {
            Eat?.Invoke();
            TPM.SetSpeed(SOMeleeAttack.speedWhileUsing);
            UsingAttackTimeLeft = 0.5f;
        }
    }

    void Shooting()
    {
        if (CurrentAmmo != null)
        {
            if (_shoot && UsingAttackTimeLeft == 0 && ConsumeAmmo())
            {
                anim.SetTrigger("ChargeSlingshot");
                TPM.SetSpeed(TPM.NormalSpeed / 2);
                Shoot?.Invoke();
                _holdingFire = true;
            }
        }

        if (_holdingFire)
        {
            UsingAttackTimeLeft = 1;

            if (_shootLastFrame && !_shoot)
            {
                anim.SetTrigger("ShootSlingshot");
                OnStopHoldShoot?.Invoke();
                UsingAttackTimeLeft = 0.2f;
                _holdingFire = false;
            }
        }
        _shootLastFrame = _shoot;
    }

    protected override void Staggered()
    {
        base.Staggered();
        OnStopHoldShoot?.Invoke();
        _holdingFire = false;
        TPM.SetSpeed(0);
    }

    public bool ConsumeAmmo()
    {
        if (CurrentAmmo.CurrentAmount > 0)
        {
            CurrentAmmo.CurrentAmount--;
            return true;
        }
        return false;
    }

    public int AvailableFruits()
    {
        int count = 0;

        foreach(SOFruit f in AmmoTypes)
        {
            if (f.CurrentAmount > 0) count++;
        }

        return count;
    }

    [ContextMenu("Refill All Ammo")]
    public void RefillAllAmmo()
    {
        foreach (SOFruit f in AmmoTypes)
        {
            f.CurrentAmount = f.MaxAmount;
        }
        if (CurrentAmmo == null) { _scroll = 1; }
    }

    public bool CollectFruit(SOFruit.Fruit f, int Amount =1)
    {
        SOFruit Fruit= AmmoTypes.Find(x => x.fruit == f); // look for fruit
        if (Fruit == null) return false; // if the fruit doesnt exist we cant pick it up
        if (Fruit.CurrentAmount < Fruit.MaxAmount) // are we full on that fruit?
        {
            Fruit.CurrentAmount += Amount;       // add the fruit amount;
            if (Fruit.CurrentAmount > Fruit.MaxAmount) Fruit.CurrentAmount= Fruit.MaxAmount; // if we carry too much then remove
            return true; // Pick up
        }
        if (CurrentAmmo == null) { _scroll = 1; }
        return false; // we full, we cant pick up any more
    }
}
