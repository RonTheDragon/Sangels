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
    [SerializeField] float AimingSpeed;
    [ReadOnly][SerializeField] float CurrentFOV;
    [SerializeField] float AimingFOV = 1;
    [ReadOnly]public bool isAiming;
    [SerializeField][Range(10, 100)] int _linePoints = 25;
    [SerializeField][Range(0.01f, 0.25f)] float _timeBetweenPoints = 0.01f;

    [Header("Ammo Switching")]
    [ReadOnly][SerializeField] FruitData CurrentAmmo;   
    [SerializeField] List<FruitData> AmmoTypes;

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
    float _fruitMass;
    Action OnStopHoldShoot;

    LayerMask LineTrajectoryMask => GM.TrajectoryHits;

    ThirdPersonMovement TPM => GetComponentInParent<ThirdPersonMovement>();
    LineRenderer LR => cinemachine.GetComponent<LineRenderer>();

    PlayerAttackManager playerAttackManager => (PlayerAttackManager)attackManager;
    CinemachineCameraOffset offset => cinemachine.GetComponent<CinemachineCameraOffset>();

    // Start is called before the first frame update
    void Start()
    {
        cam = playerAttackManager.Cam.transform;
        cinemachine = playerAttackManager.Cinemachine;

        Attackable = attackManager.Attackable;

        SwitchAmmo();

        OnStopHoldShoot += OnStoppedShooting;
        playerAttackManager.Loop += Shoot;
        playerAttackManager.Loop += Aim;
        playerAttackManager.Loop += AmmoSwitching;
    }
    

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position,cam.forward,out hit,Mathf.Infinity))
        {
            if (hit.distance < 5) { ProjectileSpawnLocation.LookAt(cam.position + cam.forward * 20); }
            else
            {
                ProjectileSpawnLocation.LookAt(hit.point);
            }
        }
        else
        {
            ProjectileSpawnLocation.LookAt(cam.position+cam.forward*200);
        }
        if (playerAttackManager._shoot && _cd <= 0 && !fruit && !string.IsNullOrEmpty(CurrentAmmo.fruit.ToString()))
        {
            fruit = ObjectPooler.Instance.SpawnFromPool(CurrentAmmo.fruit.ToString(), ProjectileSpawnLocation.position, ProjectileSpawnLocation.rotation).GetComponent<Projectile>();
            fruit.SpawnOnSlingShot(ProjectileSpawnLocation);
            CurrentCharge = StartCharge;
            _charging = true;
            Damage d = fruit.GetComponent<Damage>();
            _fruitMass = fruit.GetComponent<Rigidbody>().mass;
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
  

    void Aim()
    {
        if (playerAttackManager._shoot)
        {
            isAiming = true;
        }

        CurrentFOV = offset.m_Offset.z;
        if (isAiming)
        {
            TPM.LookAt(cam.position + cam.forward * 20);

            if (CurrentCharge > StartCharge) 
            DrawProjection();
            if (CurrentFOV < AimingFOV)
            {
                CurrentFOV += AimingSpeed * Time.deltaTime;
                offset.m_Offset.Set(0, 0, CurrentFOV);
            }
            else {
                CurrentFOV = AimingFOV;
                offset.m_Offset.Set(0, 0, CurrentFOV);
            }
            
        }
        else
        {
            TPM.LookAtReset();

            LR.enabled = false;
            if (CurrentFOV > 0)
            {
                CurrentFOV -= AimingSpeed * Time.deltaTime;
                offset.m_Offset.Set(0, 0, CurrentFOV);
            }
            else {
                CurrentFOV = 0;
                offset.m_Offset.Set(0, 0, CurrentFOV);
            }
        }
    }

    void DrawProjection()
    {
        LR.enabled = true;
        LR.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenPoints) + 1;
        Vector3 startPosition = ProjectileSpawnLocation.position;
        Vector3 startVelocity = CurrentCharge * ProjectileSpawnLocation.forward / _fruitMass;
        int i = 0;
        LR.SetPosition(i, startPosition);
        for (float time = 0; time < _linePoints; time += _timeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);
            LR.SetPosition(i, point);

            Vector3 lastPosition = LR.GetPosition(i - 1);
            if (Physics.Raycast(lastPosition, (point - lastPosition).normalized,
                out RaycastHit hit, (point - lastPosition).magnitude, LineTrajectoryMask))
            {
                LR.SetPosition(i, hit.point);
                LR.positionCount = i + 1;
                return;
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
            CurrentAmmo = null;
        }
    }

}
