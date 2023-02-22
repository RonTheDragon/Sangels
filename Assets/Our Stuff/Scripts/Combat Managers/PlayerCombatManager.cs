using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatManager : CombatManager
{
    [Header("Ammo Switching")]
    [ReadOnly] public SOFruit CurrentAmmo;
    public List<SOFruit> AmmoTypes;
    [ReadOnly] public int CurrentAmmoSlot;
    [SerializeField] private bool _refillAmmoOnStart, _resetLeavesOnStart;
    [SerializeField] private int _maxAmountOnStart = 3;

    [Header("Refrences")]
    public Camera Cam;
    public CinemachineVirtualCamera Cinemachine;
    private PlayerController _pc => GetComponentInParent<PlayerController>();

    private PlayerHealth _playerHealth => _health as PlayerHealth;

    [HideInInspector]
    public bool UseShoot;
    [HideInInspector]
    public float UseScroll;

    private bool _shootLastFrame;
    private bool _holdingFire;
    private bool _eat;
    private bool _canGetUp;

    public Action<int> OnConsumeFruitUI;
    public Action Shoot;
    public Action OnStopHoldShoot;
    public Action Eat;

    public Action<bool> UIcanGetUp;

    new protected void Start()
    {
        base.Start();

        if (_refillAmmoOnStart)
        {
            RefillAllAmmo();
        }
        if (_resetLeavesOnStart)
        {
            ResetMaxAmmo();
        }

        Attackable = _gm.PlayersCanAttack;
        Healable = _gm.PlayersOnly;

        _pc.OnStagger += Staggered;
        _pc.OnStun    += Stunned;
        _pc.OnGetUp   += GetUp;
        _playerHealth.OnRevive += StandingUp;

        Loop += Melee;
        Loop += Shooting;
        Loop += Eating;
    }

    protected override void BusyEnded()
    {
        base.BusyEnded();
        _pc.SetSpeed(_pc.NormalSpeed);
    }

    #region Inputs
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
    #endregion

    #region Activities
    private void Melee()
    {
        if (IsMelee && !CheckIfBusy())
        {
            _anim.SetTrigger(SOMeleeAttack.AnimationName);
            _pc.SetSpeed(SOMeleeAttack.SpeedWhileUsing);
            _busyTimeLeft = SOMeleeAttack.UsingTime;
        }
    }

    private void Eating()
    {
        if (CurrentAmmo == null) return;
        if (_eat && !CheckIfBusy() && ConsumeAmmo())
        {
            Eat?.Invoke();
            _pc.SetSpeed(SOMeleeAttack.SpeedWhileUsing);
            _busyTimeLeft = 0.5f;
        }
    }

    private void Shooting()
    {
        if (CurrentAmmo != null)
        {
            if (UseShoot && !CheckIfBusy() && ConsumeAmmo())
            {
                _anim.SetTrigger("ChargeSlingshot");
                _pc.SetSpeed(_pc.NormalSpeed / 2);
                Shoot?.Invoke();
                _holdingFire = true;
            }
        }

        if (_holdingFire)
        {
            _busyTimeLeft = 1;

            if (_shootLastFrame && !UseShoot)
            {
                _anim.SetTrigger("ShootSlingshot");
                OnStopHoldShoot?.Invoke();
                _busyTimeLeft = 0.2f;
                _holdingFire = false;
            }
        }
        _shootLastFrame = UseShoot;
    }
    #endregion

    #region AttackImpacts
    protected override void Staggered()
    {
        base.Staggered();
        OnStopHoldShoot?.Invoke();
        _holdingFire = false;
    }
    protected override void Stunned(float StunTime)
    {
        base.Stunned(StunTime);
        OnStopHoldShoot?.Invoke();
        _holdingFire = false;
    }

    protected override void EndStunned()
    {
        _canGetUp = true;
        UIcanGetUp?.Invoke(true);
    }

    private void GetUp()
    {
        if (_canGetUp)
        {
            _canGetUp = false;
            UIcanGetUp?.Invoke(false);
            StandingUp();
        }
    }

    #endregion

    #region Ammo
    public bool ConsumeAmmo()
    {
        if (CurrentAmmo.CurrentAmount > 0)
        {
            CurrentAmmo.CurrentAmount--;
            OnConsumeFruitUI?.Invoke(CurrentAmmo.CurrentAmount);
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
    public void ResetMaxAmmo()
    {
        foreach (SOFruit f in AmmoTypes)
        {
            f.MaxAmount = _maxAmountOnStart;
        }
    }
    #endregion

    #region Collect

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

    public void CollectLeaf(SOFruit.Fruit f)
    {
        SOFruit Fruit = AmmoTypes.Find(x => x.FruitType == f); // look for fruit
        Fruit.MaxAmount++;
    }

    #endregion
}
