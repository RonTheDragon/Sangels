using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerAttackManager : AttackManager
{
    public Camera Cam;
    public CinemachineFreeLook Cinemachine;

    [HideInInspector]
    public bool _shoot;
    [HideInInspector]
    public float _scroll;

    ThirdPersonMovement TPM => GetComponentInParent<ThirdPersonMovement>();

    private void Awake()
    {
        Attackable = GM.PlayersCanAttack;
        TPM.SetAnimator(anim);
        Loop += Melee;
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
}
