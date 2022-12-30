using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerSlingshot : Combat
{
    //Serializefield 
    [SerializeField] Transform ProjectileSpawnLocation;


    [Header("Aiming")]
    [SerializeField] float AimingSpeed;
    [ReadOnly][SerializeField] float CurrentFOV;
    [SerializeField] float AimingFOV = 1;
    [ReadOnly]public bool isAiming;
    [SerializeField][Range(10, 100)] int _linePoints = 25;
    [SerializeField][Range(0.01f, 0.25f)] float _timeBetweenPoints = 0.01f;

    [Header("Charge")]
    [SerializeField] float MaxCharge = 2000;
    [SerializeField] float StartCharge = 100;
    [SerializeField] float ChargingSpeed = 1900;
    [ReadOnly][SerializeField] float CurrentCharge;

    [Header("AimAssist")]
    [ReadOnly][SerializeField] Transform TargetAim;
    float _aimAssistRange;
    [SerializeField] LayerMask AimTriggerLM;
    [SerializeField] float _aimAssistDir;
    [SerializeField] float _aimAssistSpeed;
    [ReadOnly] [SerializeField] float _AimAssitTimeLcokOn;//timer

    //Private 
    bool _charging;

    Transform cam;
    CinemachineFreeLook cinemachine;
    Projectile fruit;
    float _fruitMass;

    LayerMask LineTrajectoryMask => GM.TrajectoryHits;

    ThirdPersonMovement TPM => GetComponentInParent<ThirdPersonMovement>();
    LineRenderer LR => cinemachine.GetComponent<LineRenderer>();

    PlayerAmmoSwitch ammoSwitch => GetComponent<PlayerAmmoSwitch>();
    PlayerCombatManager playerAttackManager => (PlayerCombatManager)attackManager;
    CinemachineCameraOffset offset => cinemachine.GetComponent<CinemachineCameraOffset>();

    [SerializeField] CinemachineFreeLook cfl;
    InputHandler _inputHandler => cfl.GetComponent<InputHandler>();


    // Start is called before the first frame update
    void Start()
    {
        attackManager.Damagers.Add(this);
        cam = playerAttackManager.Cam.transform;
        cinemachine = playerAttackManager.Cinemachine;
        playerAttackManager.Loop += Shoot;
        playerAttackManager.Loop += Aim;
        playerAttackManager.Shoot += OnStartShooting;
        playerAttackManager.OnStopHoldShoot += OnStoppedShooting;
    }
    

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position,cam.forward,out hit,Mathf.Infinity,GM.Everything,QueryTriggerInteraction.Ignore))
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
        
        if (_charging)
        {
            CurrentCharge += ChargingSpeed * Time.deltaTime;
            if (CurrentCharge > MaxCharge) CurrentCharge = MaxCharge;
        }


    }
  

    void Aim()
    {
        CurrentFOV = offset.m_Offset.z;
        if (isAiming)
        {
            TPM.LookAt(cam.position + cam.forward * 20);
            AimAssist();

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
        Vector3 startVelocity = (CurrentCharge*fruit.ForceMultiplier) * ProjectileSpawnLocation.forward / _fruitMass;
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

    public void OnStartShooting()
    {
        isAiming = true;
        if (!fruit && !string.IsNullOrEmpty(playerAttackManager.CurrentAmmo.fruit.ToString()))
        {
            fruit = ObjectPooler.Instance.SpawnFromPool(playerAttackManager.CurrentAmmo.fruit.ToString(), ProjectileSpawnLocation.position, ProjectileSpawnLocation.rotation).GetComponent<Projectile>();
            fruit.SpawnOnSlingShot(ProjectileSpawnLocation);
            CurrentCharge = StartCharge;
            _charging = true;
            ProjectileDamage d = fruit.GetComponent<ProjectileDamage>();
            d.Shooter = transform.parent.gameObject;
            _fruitMass = fruit.GetComponent<Rigidbody>().mass;
            d.Attackable = attackManager.Attackable;
        }
    }

    void OnStoppedShooting()
    {
        isAiming = false;
        _inputHandler.IsAimAssist = false;
        if (fruit)
        {
            Physics.IgnoreCollision(fruit.GetComponent<Collider>(), GetComponentInParent<Collider>());
            fruit.LaunchProjectile(CurrentCharge);
            CurrentCharge = 0;
            _charging = false;
            fruit = null;
        }
    }


    void AimAssist() 
    {
        if (CheckIfNeedsAimAssist())
        {
            Vector2 aimAssistOffset = AimAssistLock();
            _inputHandler.AimAssistOffset = aimAssistOffset;
            _inputHandler.IsAimAssist = true;
        }
        else
            _inputHandler.IsAimAssist = false;
    }

    bool CheckIfNeedsAimAssist()
    {
        RaycastHit hit;
        Debug.DrawRay(cam.position, cam.forward * _aimAssistDir, Color.black);
        if (Physics.Raycast(cam.position, cam.forward * _aimAssistDir, out hit,  _aimAssistDir, 1 << AimTriggerLM))
        {
          //  Debug.Log("Raycast has hit");
            return true;
        }
        return false;
    }
    Vector2 AimAssistLock() 
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.position, cam.forward, out hit, _aimAssistDir, AimTriggerLM,QueryTriggerInteraction.Collide))
        return Vector2.zero;
        //if (AimTriggerLM != (AimTriggerLM | (1 << hit.transform.gameObject.layer))) return Vector2.zero;
        //Debug.Log("Raycast has hit");
        Vector3 CamInYZeroX= cam.position;
        CamInYZeroX = new Vector3(CamInYZeroX.x, 0, CamInYZeroX.z);
        Vector3 ColliderInYZeroX = new Vector3(hit.collider.transform.position.x,0, hit.collider.transform.position.z);
        float y= cam.rotation.y;
        float targetAngleY = Mathf.Atan2(cam.position.z - hit.collider.transform.position.z, cam.position.x - hit.collider.transform.position.x) * Mathf.Rad2Deg + 90;
        float targetAngleX = Mathf.Atan2(Vector3.Distance(CamInYZeroX, ColliderInYZeroX), cam.position.y - hit.collider.transform.position.y) * Mathf.Rad2Deg-90;
        
        float deltaAngleCamAndTriggerY= GM.AngleDifference(targetAngleY, -cam.eulerAngles.y);
        float deltaAngleCamAndTriggerX = -GM.AngleDifference(targetAngleX, -cam.eulerAngles.x);
        //float degree=0;

        //Vector2 hitPoint = new Vector2(hit.point.x, hit.point.y);
        //  if (0<(360 - cam.localEulerAngles.x) && (360 - cam.localEulerAngles.x) < 180)
        //      degree = -(360 - cam.localEulerAngles.x);
        //  else
        //      degree =  cam.localEulerAngles.x;
        //Debug.Log($"{targetAngleX}  ");

        return new Vector2(deltaAngleCamAndTriggerY*0.01f*hit.distance, deltaAngleCamAndTriggerX * 0.01f * hit.distance);
    }

   




}
