using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIController : Controller
{
    // Visible

    [Header("Roaming")]
    [Tooltip("x = Min Cooldown\n y = Max Cooldown\n and the random picks between them")]
    [SerializeField] private Vector2 _roamCooldown = new Vector2(1, 3);
    [ReadOnly][SerializeField] private float _roamCD;
    [SerializeField] private float _patrolRange = 20;

    [Header("Scanning")]
    [SerializeField] private float _scanRadius;
    [SerializeField] private float _angleOfVision = 90;
    [SerializeField] private float _sensingRadius = 3;
    [SerializeField] private float _scanFrequent = 1;
    [ReadOnly][SerializeField] private float _scanCD;
    [ReadOnly][SerializeField] private Transform _target;

    [Header("Alertion")]
    [SerializeField] private float _alertRadius;
    [ReadOnly][SerializeField] private float _currentAlert;
    [SerializeField] private float _attackAlert = 0.5f;
    [SerializeField] private float _maxAlert = 3;

    // Invisible

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
        if(_target!=null)
            LookAt(_target.position);
    }



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
        if (_target == null)
            return false;
        else
            return true;
    }

    public void RoamCooldown()//done
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

    public void ScanCooldown()//done
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




    public void WalkAround()//done
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
                    _agent.SetDestination(transform.position + walkTo);
                }
            }
        }
        else if (_agent.isActiveAndEnabled)
            _agent.SetDestination(_spawnPoint);
    }

    public void ScanForTarget()//done
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

    public void FollowTarget()//done
    {
        if (_agent.isActiveAndEnabled)
        _agent.SetDestination(_target.position);
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
        if (Physics.Raycast(transform.position, _target.transform.position - transform.position, out hit, _alertRadius, _canSee))
        {
            if (hit.transform == _target)
            {
                AlertionRay(Color.blue);
                _currentAlert += Time.deltaTime;
                _currentAlert = _currentAlert > _maxAlert ? _maxAlert : _currentAlert;
                return;
            }
        }
        AlertionRay(Color.red);
        _currentAlert -= Time.deltaTime;
        if (_currentAlert < 0) { _target = null; }
    }

    private void AlertionRay(Color c)
    {
        Debug.DrawRay(transform.position, _target.position - transform.position, c);
    }
    public void DetectionRay()
    {
        Vector3 RightEye = new Vector3(Mathf.Sin(Mathf.Deg2Rad * _angleOfVision / 2), 0, Mathf.Cos(Mathf.Deg2Rad * _angleOfVision / 2));
        Vector3 LeftEye = new Vector3(Mathf.Sin(Mathf.Deg2Rad * -_angleOfVision / 2), 0, Mathf.Cos(Mathf.Deg2Rad * -_angleOfVision / 2));


        Debug.DrawRay(transform.position, transform.rotation * RightEye * _scanRadius, Color.green);
        Debug.DrawRay(transform.position, transform.rotation * LeftEye * _scanRadius, Color.green);
    }

    protected override void ApplyingForce()
    {
        if (_forceStrength > 0)
        {
            transform.Translate(-_forceDirection.normalized * _forceStrength * Time.deltaTime);
            _forceStrength -= 0.02f + _forceStrength * 2 * Time.deltaTime;
        }
    }

    public override void Hurt(GameObject Attacker = null, float Staggered = 1)
    {
        base.Hurt(Attacker,Staggered);
        if (_target == null && Attacker != null) //|| Staggered && Attacker != null)
        {
            SetTarget(Attacker.transform);
            _currentAlert = _maxAlert;
        }
    }

    private void SetTarget(Transform target)
    {
        if(target.GetComponent<PlayerHealth>().IsDead)
        {
            target = null;
            return;
        }
        _target = target;
        AiAtackManager.Target = target;
    }
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

}


