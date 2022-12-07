using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerSlingshot : Damage
{
    //Serializefield 
    [Tooltip("shooting cooldown")]
    [SerializeField] float CoolDown = 0.5f;
    [SerializeField] Transform ProjectileSpawnLocation;


    [Header("Aiming")]
    [SerializeField] float AimingFOV =40;
    [SerializeField] float NotAimingFOV=70;
    [ReadOnly][SerializeField] float FovChangingSpeed;
    [SerializeField] float XScreenNotAiming=0.2f;
    [SerializeField] float XScreenAiming=0.1f;
    [ReadOnly][SerializeField] float XChangingSpeed;
    [SerializeField] float AimingSpeed;
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
    float _cd;
    int _currentAmmo;
    bool _charging, _shootLastFrame,_switchUp,_switchDown;

    Transform cam;
    CinemachineFreeLook cinemachine;
    Projectile fruit;
    Action OnStopHoldShoot;
    //MeleeDamage md => GetComponent<MeleeDamage>();

    PlayerAttackManager playerAttackManager => (PlayerAttackManager)attackManager;

    // Start is called before the first frame update
    void Start()
    {
        cam = playerAttackManager.Cam.transform;
        cinemachine = playerAttackManager.Cinemachine;

        Attackable = attackManager.Attackable;
        cinemachine.m_Lens.FieldOfView = NotAimingFOV;

        SwitchAmmo();
        CalculateAimSpeed();

        OnStopHoldShoot += OnStoppedShooting;
        playerAttackManager.Loop += Shoot;
        playerAttackManager.Loop += Aim;
        playerAttackManager.Loop += AmmoSwitching;
    }
    
    void FieldOdViewChanger(float fov,bool IsAdding) 
    {
        if(IsAdding)
            for (int i = 0; i <=2; i++)
                cinemachine.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_ScreenX +=fov;
        else 
            for (int i = 0; i <= 2; i++)
                cinemachine.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_ScreenX = fov;

            
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
        if (playerAttackManager._shoot && _cd <= 0 && !fruit && !string.IsNullOrEmpty(CurrentAmmo))
        {
            fruit = ObjectPooler.Instance.SpawnFromPool(CurrentAmmo, ProjectileSpawnLocation.position, ProjectileSpawnLocation.rotation).GetComponent<Projectile>();
            fruit.SpawnOnSlingShot(ProjectileSpawnLocation);
            CurrentCharge = StartCharge;
            _charging = true;
            Damage d = fruit.GetComponent<Damage>();
            d.Attackable = Attackable;
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
        
        if(_shootLastFrame && !playerAttackManager._shoot)
        {
            OnStopHoldShoot?.Invoke();
        }
        _shootLastFrame = playerAttackManager._shoot;
    }

    [ContextMenu("Caclculate Speed")]
    void CalculateAimSpeed() 
    {
        FovChangingSpeed = (NotAimingFOV - AimingFOV) * AimingSpeed;
        XChangingSpeed = (XScreenNotAiming - XScreenAiming)*AimingSpeed;
    }
        


    void Aim()
    {
        if (playerAttackManager._shoot)
        {
            isAiming = true;
        }

        CurrentFOV = cinemachine.m_Lens.FieldOfView;
        if (isAiming)
        {
            if (CurrentFOV > AimingFOV)
            {
                cinemachine.m_Lens.FieldOfView -= FovChangingSpeed * Time.deltaTime;
                FieldOdViewChanger(-XChangingSpeed * Time.deltaTime,true);
            }
            else { cinemachine.m_Lens.FieldOfView = AimingFOV;
                FieldOdViewChanger(XScreenAiming,false);
            }

        }
        else
        {
            if (CurrentFOV < NotAimingFOV)
            {
                cinemachine.m_Lens.FieldOfView += FovChangingSpeed * Time.deltaTime;
                FieldOdViewChanger(XChangingSpeed*Time.deltaTime,true);
            }
            else { cinemachine.m_Lens.FieldOfView = NotAimingFOV;
                FieldOdViewChanger(XScreenNotAiming,false);
            }
        }
    }

    void OnStoppedShooting()
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
            if (playerAttackManager._scroll > 0)
            {
                _currentAmmo++;
                if (_currentAmmo > AmmoTypes.Count-1)
                {
                    _currentAmmo = 0;
                }
                    SwitchAmmo();
            }
            else if (playerAttackManager._scroll < 0)
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
            playerAttackManager._scroll = 0;

            //Debug.Log(CurrentAmmo);
        }
        else
        {
            CurrentAmmo = string.Empty;
        }
    }

}
