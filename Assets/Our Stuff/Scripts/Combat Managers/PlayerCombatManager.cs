using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerCombatManager : CombatManager
{
    public Camera Cam;
    public CinemachineFreeLook Cinemachine;

    [HideInInspector]
    public bool _shoot;
    [HideInInspector]
    public float _scroll;

    bool _shootLastFrame;
    bool _holdingFire;
    bool _eat;

    ThirdPersonMovement TPM => GetComponentInParent<ThirdPersonMovement>();

    public Action Shoot;
    public Action OnStopHoldShoot;
    public Action Eat;

    private void Start()
    {
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
        TPM.ChangeSpeed(TPM.NormalSpeed);
    }

    void Melee()
    {
        if (_melee && UsingAttackTimeLeft==0)
        {
            OverrideToAttack();
            anim.SetTrigger(SOMeleeAttack.AnimationName);
            TPM.ChangeSpeed(SOMeleeAttack.speedWhileUsing);
            UsingAttackTimeLeft = SOMeleeAttack.UsingTime;
        }
    }

    void Eating()
    {
        if (_eat && UsingAttackTimeLeft == 0)
        {
            OverrideToAttack();
            Eat?.Invoke();
            TPM.ChangeSpeed(SOMeleeAttack.speedWhileUsing);
            UsingAttackTimeLeft = 0.5f;
        }
    }

    void Shooting()
    {
        if (_shoot && UsingAttackTimeLeft == 0)
        {
            OverrideToAttack();
            anim.SetTrigger("ChargeSlingshot");
            TPM.ChangeSpeed(TPM.NormalSpeed/2);
            Shoot?.Invoke();
            _holdingFire = true;
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
        TPM.ChangeSpeed(0);
    }
}
