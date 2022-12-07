using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackManager : AttackManager
{
    public Camera Cam;
    public CinemachineFreeLook Cinemachine;

    [HideInInspector]
    public bool _shoot;
    [HideInInspector]
    public float _scroll;

    GameManager GM = GameManager.instance;

    private void Awake()
    {
        Attackable = GM.PlayersCanAttack;      
    }

    // Update is called once per frame
    void Update()
    {
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

}
