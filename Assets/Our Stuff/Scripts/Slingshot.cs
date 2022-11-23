using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [ReadOnly][SerializeField] bool isAiming;

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
    bool _charging;
    Projectile fruit;

    // Start is called before the first frame update
    void Start()
    {
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
        if (Input.GetMouseButtonDown(1) && _cd <= 0 && !fruit)
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
        if (Input.GetMouseButtonUp(1)&&fruit)
        {
            fruit.LaunchProjectile(CurrentCharge);
            CurrentCharge = 0;
            _charging = false;
            _cd = CoolDown;
            fruit = null;
        }
        if (_cd > 0)
        {
            _cd -= Time.deltaTime;
        }
    }
    void Aim()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
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

    void AmmoSwitching()
    {
        if (AmmoTypes.Count > 1)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                _currentAmmo++;
                if (_currentAmmo > AmmoTypes.Count-1)
                {
                    _currentAmmo = 0;
                    SwitchAmmo();
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                _currentAmmo--;
                if (_currentAmmo < 0)
                {
                    _currentAmmo = AmmoTypes.Count-1;
                    SwitchAmmo();
                }
            }
        }
    }

    void SwitchAmmo()
    {
        if (AmmoTypes.Count > 0)
        {
            CurrentAmmo = AmmoTypes[_currentAmmo];
        }
        else
        {
            CurrentAmmo = string.Empty;
        }
    }

}
