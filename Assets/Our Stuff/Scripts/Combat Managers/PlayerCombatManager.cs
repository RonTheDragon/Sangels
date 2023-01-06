using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerCombatManager : CombatManager
{
    [Header("Ammo Switching")]
    [ReadOnly] public SOFruit CurrentAmmo;
    public List<SOFruit> AmmoTypes;
    [ReadOnly] public int CurrentAmmoSlot;
    public bool RefillAmmoOnStart;

    [Header("Refrences")]
    public Camera Cam;
    public CinemachineFreeLook Cinemachine;
    private PlayerController _pc => GetComponentInParent<PlayerController>();

    [HideInInspector]
    public bool UseShoot;
    [HideInInspector]
    public float UseScroll;

    private bool _shootLastFrame;
    private bool _holdingFire;
    private bool _eat;

    public Action Shoot;
    public Action OnStopHoldShoot;
    public Action Eat;

    private void Start()
    {
        if (RefillAmmoOnStart)
        {
            RefillAllAmmo();
        }

        Attackable = _gm.PlayersCanAttack;
        Loop += Melee;
        Loop += Shooting;
        Loop += Eating;
        _pc.OnStagger += Staggered;
    }

    // Update is called once per frame
    new private void Update()
    {
        base.Update();
        Loop?.Invoke();
    }



    //Inputs
    public void OnShoot(InputAction.CallbackContext context)
    {
        UseShoot = context.action.triggered;
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        base.IsMelee = context.action.triggered;
    }

    public void OnEat(InputAction.CallbackContext context)
    {
        _eat = context.action.triggered;
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        UseScroll = context.action.ReadValue<float>();
    }

    protected override void AttackEnded()
    {
        _pc.SetSpeed(_pc.NormalSpeed);
    }

    private void Melee()
    {
        if (IsMelee && _usingAttackTimeLeft == 0)
        {
            Anim.SetTrigger(SOMeleeAttack.AnimationName);
            _pc.SetSpeed(SOMeleeAttack.SpeedWhileUsing);
            _usingAttackTimeLeft = SOMeleeAttack.UsingTime;
        }
    }

    private void Eating()
    {
        if (CurrentAmmo == null) return;
        if (_eat && _usingAttackTimeLeft == 0 && ConsumeAmmo())
        {
            Eat?.Invoke();
            _pc.SetSpeed(SOMeleeAttack.SpeedWhileUsing);
            _usingAttackTimeLeft = 0.5f;
        }
    }

    private void Shooting()
    {
        if (CurrentAmmo != null)
        {
            if (UseShoot && _usingAttackTimeLeft == 0 && ConsumeAmmo())
            {
                Anim.SetTrigger("ChargeSlingshot");
                _pc.SetSpeed(_pc.NormalSpeed / 2);
                Shoot?.Invoke();
                _holdingFire = true;
            }
        }

        if (_holdingFire)
        {
            _usingAttackTimeLeft = 1;

            if (_shootLastFrame && !UseShoot)
            {
                Anim.SetTrigger("ShootSlingshot");
                OnStopHoldShoot?.Invoke();
                _usingAttackTimeLeft = 0.2f;
                _holdingFire = false;
            }
        }
        _shootLastFrame = UseShoot;
    }

    protected override void Staggered()
    {
        base.Staggered();
        OnStopHoldShoot?.Invoke();
        _holdingFire = false;
        _pc.SetSpeed(0);
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
        if (CurrentAmmo == null) { UseScroll = 1; }
    }

    public bool CollectFruit(SOFruit.Fruit f, int Amount =1)
    {
        SOFruit Fruit= AmmoTypes.Find(x => x.FruitType == f); // look for fruit
        if (Fruit == null) return false; // if the fruit doesnt exist we cant pick it up
        if (Fruit.CurrentAmount < Fruit.MaxAmount) // are we full on that fruit?
        {
            Fruit.CurrentAmount += Amount;       // add the fruit amount;
            if (Fruit.CurrentAmount > Fruit.MaxAmount) Fruit.CurrentAmount= Fruit.MaxAmount; // if we carry too much then remove
            return true; // Pick up
        }
        if (CurrentAmmo == null) { UseScroll = 1; }
        return false; // we full, we cant pick up any more
    }
}
