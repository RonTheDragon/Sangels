using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Slingshot : MonoBehaviour
{
    //Serializefield 
    [Tooltip("shooting cooldown")]
    [SerializeField] float CoolDown = 0.5f;
    [SerializeField] Transform ProjectileSpawnLocation;
    [SerializeField] Transform cam;
    [SerializeField] CinemachineFreeLook cinemachine;

    [Header("Aiming")]
    [SerializeField] float AimingFOV =40;
    [SerializeField] float NotAimingFOV=70;
    [SerializeField] float FovChangingSpeed = 60;
    [ReadOnly][SerializeField] float CurrentFOV;
    [ReadOnly]public bool isAiming;

    [Header("Ammo Switching")]
    [ReadOnly][SerializeField] string CurrentAmmo;
    [SerializeField] List<string> AmmoTypes;

    [Header("Charge")]
    [SerializeField] float MaxCharge = 2000;
    [SerializeField] float StartCharge = 100;
    [SerializeField] float ChargingSpeed = 1900;
    [ReadOnly][SerializeField] float CurrentCharge;

    //Private 
    float _cd, _scroll;
    int _currentAmmo;
    bool _charging, _shoot, _shootLastFrame,_switchUp,_switchDown;
    Projectile fruit;
    event EventHandler OnStopHoldShoot;

    // Start is called before the first frame update
    void Start()
    {
        OnStopHoldShoot += OnStoppedShooting;
        cinemachine.m_Lens.FieldOfView = NotAimingFOV;
        SwitchAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
        Aim();
        AmmoSwitching();
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position,cam.forward,out hit,Mathf.Infinity))
        {
            ProjectileSpawnLocation.LookAt(hit.point);
        }
        else
        {
            ProjectileSpawnLocation.LookAt(cam.position+cam.forward*200);
        }
        if (_shoot && _cd <= 0 && !fruit && !string.IsNullOrEmpty(CurrentAmmo))
        {
            fruit = ObjectPooler.Instance.SpawnFromPool(CurrentAmmo, ProjectileSpawnLocation.position, ProjectileSpawnLocation.rotation).GetComponent<Projectile>();
            fruit.SpawnOnSlingShot(ProjectileSpawnLocation);
            CurrentCharge = StartCharge;
            _charging = true;
        }
        if (_charging)
        {
            CurrentCharge += ChargingSpeed * Time.deltaTime;
            if (CurrentCharge > MaxCharge) CurrentCharge = MaxCharge;
        }
       
        if (_cd > 0)
        {
            _cd -= Time.deltaTime;
        }
        
        if(_shootLastFrame && !_shoot)
        {
            OnStopHoldShoot?.Invoke(this, EventArgs.Empty);
        }
        _shootLastFrame = _shoot;
    }
    void Aim()
    {
        if (_shoot)
        {
            isAiming = true;
        }

        CurrentFOV = cinemachine.m_Lens.FieldOfView;
        if (isAiming)
        {
            if (CurrentFOV > AimingFOV)
            {
                cinemachine.m_Lens.FieldOfView -= FovChangingSpeed * Time.deltaTime;
            }
            else { cinemachine.m_Lens.FieldOfView = AimingFOV; }

        }
        else
        {
            if (CurrentFOV < NotAimingFOV)
            {
                cinemachine.m_Lens.FieldOfView += FovChangingSpeed * Time.deltaTime;
            }
            else { cinemachine.m_Lens.FieldOfView = NotAimingFOV; }
        }
    }

    void OnStoppedShooting(object sender,EventArgs eventArgs)
    {
        isAiming = false;
        if (fruit)
        {
            fruit.LaunchProjectile(CurrentCharge);
            CurrentCharge = 0;
            _charging = false;
            _cd = CoolDown;
            fruit = null;
        }
    }

    void AmmoSwitching()
    {
        if (AmmoTypes.Count > 1)
        {
            if (_scroll > 0)
            {
                _currentAmmo++;
                if (_currentAmmo > AmmoTypes.Count-1)
                {
                    _currentAmmo = 0;
                }
                    SwitchAmmo();
            }
            else if (_scroll < 0)
            {
                _currentAmmo--;
                if (_currentAmmo < 0)
                {
                    _currentAmmo = AmmoTypes.Count-1;
                }
                    SwitchAmmo();
            }
        }
    }

    void SwitchAmmo()
    {
        if (AmmoTypes.Count > 0)
        {
            CurrentAmmo = AmmoTypes[_currentAmmo];
            _scroll = 0;

            //Debug.Log(CurrentAmmo);
        }
        else
        {
            CurrentAmmo = string.Empty;
        }
    }

    //Inputs
    public void OnShoot(InputAction.CallbackContext context)
    {
       _shoot= context.action.triggered;
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        _scroll = context.action.ReadValue<float>();
       // Debug.Log(_scroll);
    }

}
