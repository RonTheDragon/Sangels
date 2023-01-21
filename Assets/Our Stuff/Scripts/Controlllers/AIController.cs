using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class AIController : Controller
{
    // Visible

    [Header("Roaming")]
    [Tooltip("x = Min Cooldown\n y = Max Cooldown\n and the random picks between them")]
    [SerializeField] private Vector2 _roamCooldown = new Vector2(1, 3);
    [ReadOnly][SerializeField] private float _roamCD;
    [SerializeField] private float _patrolRange = 20;
    [SerializeField] private float _returningToNavMeshRange = 10;

    [Header("Scanning")]
    [SerializeField] private float _scanRadius;
    [SerializeField] private float _angleOfVision = 90;
    [SerializeField] private float _sensingRadius = 3;
    [SerializeField] private float _scanFrequent = 1;
    [ReadOnly][SerializeField] private float _scanCD;
    [ReadOnly] public Transform Target;
    private Health _targetHealth;

    [Header("Alertion")]
    [SerializeField] private float _alertRadius;
    [ReadOnly][SerializeField] private float _currentAlert;
    [SerializeField] private float _attackAlert = 0.5f;
    [SerializeField] private float _maxAlert = 3;


    // Invisible

    //Lure
    private Vector3 _lureDestination;
    public float LuredTime;

    //Layer Masks
    private LayerMask _canSee;
    private LayerMask _attackable;


    // Refrences
    public AiCombatManager AiAtackManager => GetComponentInChildren<AiCombatManager>();
    private GameManager _gm => GameManager.Instance;
    private Vector3 _spawnPoint => transform.position;
    private NavMeshAgent _agent => GetComponent<NavMeshAgent>();


    new private void Start()
    {
        base.Start();
        _canSee = _gm.EnemiesCanSee;
        _attackable = _gm.EnemiesCanAttack;
        _anim.SetBool("Walking", true);
    }

    new private void Update()
    {
        base.Update();
        if(Target!=null && !_characterHealth.IsStunned)
            LookAt(Target.position);
    }


    #region Return bools for State Machine
    public bool IsAlertAttack() 
    {
        AlertSystem();
        if (_currentAlert > _attackAlert)
            return true;
        else
            return false;
    }
    public bool HasTarget() 
    {
        if (Target == null)
            return false;
        else
            return true;
    }
    #endregion

    #region Cooldowns
    public void RoamCooldown()
    {
            if (_roamCD <= 0)
            {
                _roamCD = Random.Range(_roamCooldown.x, _roamCooldown.y);
                WalkAround();
            }
            else
            {
                 _roamCD -= Time.deltaTime;
            }
    }

    public void ScanCooldown()
    {

        if (_scanCD <= 0)
        {
            ScanForTarget();
            _scanCD = _scanFrequent;
        }
        else
        {
            _scanCD -= Time.deltaTime;
        }
    }

    public void LuredCooldown()
    {
        if (LuredTime > 0)
        {
            SetDestination(_lureDestination);
            LuredTime -= Time.deltaTime;
        }
    }
    #endregion

    #region Movement
    /// <summary>
    /// Roam State Movement
    /// </summary>
    public void WalkAround()
    {
        if (Vector3.Distance(_spawnPoint, transform.position) < _patrolRange)
        {
            Vector3 walkTo = new Vector3();
            for (int i = 0; i < 10; i++)
            {
                walkTo.x = Random.Range(-10, 10);
                walkTo.z = Random.Range(-10, 10);
                if (_patrolRange > Vector3.Distance(_spawnPoint, transform.position + walkTo) && _agent.isActiveAndEnabled)//in range
                {
                    SetDestination(transform.position + walkTo);
                }
            }
        }
        else if (_agent.isActiveAndEnabled)
            SetDestination(_spawnPoint);
    }

    /// <summary>
    /// Follow State Movement
    /// </summary>
    public bool FollowTarget()
    {
        if (_targetHealth != null)
        {
            if (!_targetHealth.IsDead)
            {
                if (_agent.isActiveAndEnabled)
                    SetDestination(Target.position);
                    return true;
                
            }
            else
            {
                Target = null;
                _targetHealth= null;
                
            }
        }
        return false;
    }

    /// <summary>
    /// Use This instead of _agent.SetDestination so the AI can teleport back to the Nav mesh rather than getting stuck
    /// </summary>
    /// <param name="pos">The Position The Agent Is Trying To Reach</param>
    public void SetDestination(Vector3 pos)
    {
        if (CheckIfCanMove())
        {
            _agent.isStopped = false;
            NavMeshHit hit;
            if (!NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                if (NavMesh.SamplePosition(transform.position, out hit, _returningToNavMeshRange, NavMesh.AllAreas))
                {
                    _agent.Warp(hit.position);
                }
                else return;
            }
            _agent.SetDestination(pos);
        }
    }

    public void StopMoving()
    {
        _agent.isStopped=true;
    }

    #endregion

    #region Detection Systems
    public void ScanForTarget()
    {
        List<Collider> colliders = Physics.OverlapSphere(transform.position, _scanRadius, _attackable).ToList();
        if (colliders == null)
            return;
       
        int colliderCount = colliders.Count;
        for (int i = 0; i < colliderCount; i++)
        {
            if (!CheckIfInFront(colliders[i].transform.position)// if not in front of player
                && Vector3.Distance(transform.position, colliders[i].transform.position)> _sensingRadius) 
            {
                colliders.Remove(colliders[i]);
                colliderCount--;
                i--;
            }
        }
        colliderCount = colliders.Count;
        for (int i = 0; i < colliderCount; i++)
        {


            Collider c = _gm.ClosestColliderInList(colliders);//if doesnt work, lets check if its the same object.

            RaycastHit hit;
            if (Physics.Raycast(transform.position, c.transform.position - transform.position, out hit, _scanRadius, _canSee))
            {
                if (hit.collider == c)
                {
                    SetTarget(c.transform);
                    _currentAlert = 0;
                    return;
                }
            }
            colliders.Remove(c);
        }
    }

    
    private bool CheckIfInFront(Vector3 pos)
    {

        float targetAngle = Mathf.Atan2(transform.position.z- pos.z, transform.position.x - pos.x) * Mathf.Rad2Deg +90 ;
        float deltaAngleAIAndTarget = _gm.AngleDifference(targetAngle, -transform.eulerAngles.y);
        //Debug.Log($"{targetAngle} , {-transform.eulerAngles.y} = {deltaAngleAIAndTarget}");
        if (deltaAngleAIAndTarget < _angleOfVision/2 && deltaAngleAIAndTarget > -_angleOfVision/2)
        {
            return true;
        }
            return false;
    }

    public void AlertSystem() 
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Target.transform.position - transform.position, out hit, _alertRadius, _canSee))
        {
            if (hit.transform == Target)
            {
                AlertionRay(Color.blue);
                _currentAlert += Time.deltaTime;
                _currentAlert = _currentAlert > _maxAlert ? _maxAlert : _currentAlert;
                return;
            }
        }
        AlertionRay(Color.red);
        _currentAlert -= Time.deltaTime;
        if (_currentAlert < 0) { Target = null; }
    }

    private void AlertionRay(Color c)
    {
        Debug.DrawRay(transform.position, Target.position - transform.position, c);
    }
    public void DetectionRay()
    {
        Vector3 RightEye = new Vector3(Mathf.Sin(Mathf.Deg2Rad * _angleOfVision / 2), 0, Mathf.Cos(Mathf.Deg2Rad * _angleOfVision / 2));
        Vector3 LeftEye = new Vector3(Mathf.Sin(Mathf.Deg2Rad * -_angleOfVision / 2), 0, Mathf.Cos(Mathf.Deg2Rad * -_angleOfVision / 2));


        Debug.DrawRay(transform.position, transform.rotation * RightEye * _scanRadius, Color.green);
        Debug.DrawRay(transform.position, transform.rotation * LeftEye * _scanRadius, Color.green);
    }
    #endregion

    public void SetLure(Vector3 pos, float time)
    {
        _lureDestination = pos;
        LuredTime = time;
    }



    protected override void ApplyingForce() //Applying Force for Navmesh Agent
    {
        if (_forceStrength > 0)
        {
            transform.Translate(-_forceDirection.normalized * _forceStrength * Time.deltaTime);
            _forceStrength -= 0.02f + _forceStrength * 2 * Time.deltaTime;
        }
    }

    public override void Hurt(CharacterHealth.EffectFromImpactType impactType, float recievedStagger, float staggerResistance, GameObject attacker = null) 
    { 
        base.Hurt(impactType, recievedStagger, staggerResistance, attacker);
        if (Target == null && attacker != null) 
        {
            SetTarget(attacker.transform);
            _currentAlert = _maxAlert;
        }
    }

    /// <summary>
    /// Attempting To Set a Target if he is Valid
    /// </summary>
    /// <param name="target"> The Target (The Player for Example) </param>
    private void SetTarget(Transform target)
    {
        _targetHealth = target.GetComponent<Health>();
        if (_targetHealth != null)
        {
            if (_targetHealth.IsDead)
            {
                target = null;
                return;
            }
        }
        else
        {
            target = null;
            return;
        }
        Target = target;
    }

    #region Speed Get Set
    public override float GetSpeed()
    {
        return Speed * (1 - (_glubCurrentEffect / (_glubMax + (_glubMax / 30))));
    }

    public override void SetSpeed(float speed = -1)
    {
        if (speed != -1)
        {
            Speed = speed;
        }
        _agent.speed = GetSpeed();
        _anim.SetFloat("Speed", GetSpeed() / RegularAnimationSpeed);

    }
    #endregion

    

}


