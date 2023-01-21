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
    [SerializeField] private Transform _projectileSpawnLocation;

    [Header("Aiming")]
    [SerializeField] private float _aimingSpeed;
    [ReadOnly][SerializeField] private float _currentFOV;
    [SerializeField] private float _aimingFOV = 1;
    [ReadOnly] public bool IsAiming;
    [SerializeField][Range(10, 100)] private int _linePoints = 25;
    [SerializeField][Range(0.01f, 0.25f)] private float _timeBetweenPoints = 0.01f;

    [Header("Charge")]
    [SerializeField] private float _maxCharge = 40;
    [SerializeField] private float _startCharge = 2;
    [SerializeField] private float _chargingSpeed = 38;
    [ReadOnly][SerializeField] private float _currentCharge;

    [Header("AimAssist")]
    [ReadOnly][SerializeField] private Transform _targetAim;
    [SerializeField] private float _aimAssistRange = 30;
    [SerializeField] private LayerMask _aimTriggerLM;
    [SerializeField] private float _aimAssistDir;
    [SerializeField] private float _aimAssistSpeed;
    [ReadOnly] [SerializeField] private float _AimAssitTimeLockOn;//timer

    //Private 
    private bool _charging;

    private Transform _cam;
    private CinemachineFreeLook _cinemachine;
    private Projectile _fruit;
    private float _fruitMass;

    private LayerMask _lineTrajectoryMask => _gameManager.TrajectoryHits;

    private PlayerController _playerController => GetComponentInParent<PlayerController>();
    private LineRenderer _line => _cinemachine.GetComponent<LineRenderer>();
    private PlayerCombatManager _playerAttackManager => (PlayerCombatManager)_attackManager;
    private CinemachineCameraOffset _offset => _cinemachine.GetComponent<CinemachineCameraOffset>();

    [SerializeField] private CinemachineFreeLook _cfl;
    private InputHandler _inputHandler => _cfl.GetComponent<InputHandler>();


    // Start is called before the first frame update
    private void Start()
    {
        _attackManager.Damagers.Add(this);
        _cam = _playerAttackManager.Cam.transform;
        _cinemachine = _playerAttackManager.Cinemachine;
        _playerAttackManager.Loop += Shoot;
        _playerAttackManager.Loop += Aim;
        _playerAttackManager.Shoot += OnStartShooting;
        _playerAttackManager.OnStopHoldShoot += OnStoppedShooting;
    }


    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(_cam.position,_cam.forward,out hit,Mathf.Infinity,_gameManager.Everything,QueryTriggerInteraction.Ignore))
        {
            if (hit.distance < 5) { _projectileSpawnLocation.LookAt(_cam.position + _cam.forward * 20); }
            else
            {
                _projectileSpawnLocation.LookAt(hit.point);
            }
        }
        else
        {
            _projectileSpawnLocation.LookAt(_cam.position+_cam.forward*200);
        }
        
        if (_charging)
        {
            _currentCharge += _chargingSpeed * Time.deltaTime;
            if (_currentCharge > _maxCharge) _currentCharge = _maxCharge;
        }


    }


    private void Aim()
    {
        _currentFOV = _offset.m_Offset.z;
        if (IsAiming)
        {
            _playerController.LookAt(_cam.position + _cam.forward * 20);
            AimAssist();

            if (_currentCharge > _startCharge) 
            DrawProjection();
            if (_currentFOV < _aimingFOV)
            {
                _currentFOV += _aimingSpeed * Time.deltaTime;
                _offset.m_Offset.Set(0, 0, _currentFOV);
            }
            else {
                _currentFOV = _aimingFOV;
                _offset.m_Offset.Set(0, 0, _currentFOV);
            }
        }
        else
        {
            _playerController.LookAtReset();

            _line.enabled = false;
            if (_currentFOV > 0)
            {
                _currentFOV -= _aimingSpeed * Time.deltaTime;
                _offset.m_Offset.Set(0, 0, _currentFOV);
            }
            else {
                _currentFOV = 0;
                _offset.m_Offset.Set(0, 0, _currentFOV);
            }
        }
    }

    private void DrawProjection()
    {
        _line.enabled = true;
        _line.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenPoints) + 1;
        Vector3 startPosition = _projectileSpawnLocation.position;
        Vector3 startVelocity = (_currentCharge*_fruit.ForceMultiplier) * _projectileSpawnLocation.forward / _fruitMass;
        int i = 0;
        _line.SetPosition(i, startPosition);
        for (float time = 0; time < _linePoints; time += _timeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);
            _line.SetPosition(i, point);

            Vector3 lastPosition = _line.GetPosition(i - 1);
            if (Physics.Raycast(lastPosition, (point - lastPosition).normalized,
                out RaycastHit hit, (point - lastPosition).magnitude, _lineTrajectoryMask))
            {
                _line.SetPosition(i, hit.point);
                _line.positionCount = i + 1;
                return;
            }
        }
    }

    public void OnStartShooting()
    {
        IsAiming = true;
        if (!_fruit && !string.IsNullOrEmpty(_playerAttackManager.CurrentAmmo.FruitType.ToString()))
        {
            _fruit = ObjectPooler.Instance.SpawnFromPool(_playerAttackManager.CurrentAmmo.FruitType.ToString(), _projectileSpawnLocation.position, _projectileSpawnLocation.rotation).GetComponent<Projectile>();
            _fruit.SpawnOnSlingshot(_projectileSpawnLocation);
            _currentCharge = _startCharge;
            _charging = true;
            _fruitMass = _fruit.GetComponent<Rigidbody>().mass;

            ProjectileDamage d = _fruit.GetComponent<ProjectileDamage>();
            if (d != null)
            {
                d.Shooter = transform.parent.gameObject;
                d.Attackable = _attackManager.Attackable;
            }
            ExplosiveHealing h = _fruit.GetComponent<ExplosiveHealing>();
            if (h != null)
            {
                h.Shooter = transform.parent.gameObject;
                h.LureMask = _attackManager.Attackable;
                h.HealMask = _attackManager.Healable;
            }
        }
    }

    private void OnStoppedShooting()
    {
        IsAiming = false;
        _inputHandler.IsAimAssist = false;
        if (_fruit)
        {
            Physics.IgnoreCollision(_fruit.GetComponent<Collider>(), GetComponentInParent<Collider>());
            _fruit.LaunchProjectile(_currentCharge);
            _currentCharge = 0;
            _charging = false;
            _fruit = null;
        }
    }


    private void AimAssist() 
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

    private bool CheckIfNeedsAimAssist()
    {
        RaycastHit hit;
        Debug.DrawRay(_cam.position, _cam.forward * _aimAssistDir, Color.black);
        if (Physics.Raycast(_cam.position, _cam.forward * _aimAssistDir, out hit,  _aimAssistDir, 1 << _aimTriggerLM))
        {
          //  Debug.Log("Raycast has hit");
            return true;
        }
        return false;
    }
    private Vector2 AimAssistLock() 
    {
        RaycastHit hit;
        if (!Physics.Raycast(_cam.position, _cam.forward, out hit, _aimAssistRange, _aimTriggerLM,QueryTriggerInteraction.Collide))
        return Vector2.zero;
        //if (AimTriggerLM != (AimTriggerLM | (1 << hit.transform.gameObject.layer))) return Vector2.zero;
        //Debug.Log("Raycast has hit");
        Vector3 CamInYZeroX= _cam.position;
        CamInYZeroX = new Vector3(CamInYZeroX.x, 0, CamInYZeroX.z);
        Vector3 ColliderInYZeroX = new Vector3(hit.collider.transform.position.x,0, hit.collider.transform.position.z);
        float y= _cam.rotation.y;
        float targetAngleY = Mathf.Atan2(_cam.position.z - hit.collider.transform.position.z, _cam.position.x - hit.collider.transform.position.x) * Mathf.Rad2Deg + 90;
        float targetAngleX = Mathf.Atan2(Vector3.Distance(CamInYZeroX, ColliderInYZeroX), _cam.position.y - hit.collider.transform.position.y) * Mathf.Rad2Deg-90;
        
        float deltaAngleCamAndTriggerY= _gameManager.AngleDifference(targetAngleY, -_cam.eulerAngles.y);
        float deltaAngleCamAndTriggerX = -_gameManager.AngleDifference(targetAngleX, -_cam.eulerAngles.x);
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
