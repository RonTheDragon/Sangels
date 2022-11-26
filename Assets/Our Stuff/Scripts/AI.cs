using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AI : MonoBehaviour
{
    // Visible

    [Header("Roaming")]
    [Tooltip("x = Min Cooldown\n y = Max Cooldown\n and the random picks between them")]
    [SerializeField] Vector2 _roamCooldown = new Vector2(1, 3);
#if UNITY_EDITOR
    [ReadOnly]
#endif
    [SerializeField] float _roamCD;
    [SerializeField] float _patrolRange = 20;

    [Header("Scanning")]
    [SerializeField] float _scanRadius;
    [SerializeField] float _angleOfVision = 90;
    [SerializeField] float ScanFrequent = 1;
#if UNITY_EDITOR
    [ReadOnly]
#endif
    [SerializeField] float _scanCD;
#if UNITY_EDITOR
    [ReadOnly]
#endif
    [SerializeField] Transform Target;

    [Header("Alertion")]
    [SerializeField] float AlertRadius;
#if UNITY_EDITOR
    [ReadOnly]
#endif
    [SerializeField] float CurrentAlert;
    [SerializeField] float AttackAlert = 0.5f;
    [SerializeField] float MaxAlert = 3;

    // Invisible

    //Layer Masks
    LayerMask _canSee;
    LayerMask _attackable;

    // Refrences
    GameManager GM => GameManager.instance;
    Vector3 spawnPoint=> transform.position;
    NavMeshAgent agent => GetComponent<NavMeshAgent>();


    private void Start()
    {
        _canSee = GM.EnemiesCanSee;
        _attackable = GM.EnemiesCanAttack;
    }

    void Update()
    {
        AIbrain();
    }

    void AIbrain()
    {


        if (Target != null)
        {
            AlertSystem();
        }
        else
        {
            DetectionRay();
            ScanCooldown();
        }

        if (CurrentAlert > AttackAlert && Target != null) 
        {           
            FollowTaget();
        }
        else
        {
            RoamCooldown();
        }

    }

    void RoamCooldown()
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

    void ScanCooldown()
    {

        if (_scanCD <= 0)
        {
            ScanForTarget();
            _scanCD = ScanFrequent;
        }
        else
        {
            _scanCD -= Time.deltaTime;
        }
    }




    void WalkAround()
    {
        if (Vector3.Distance(spawnPoint, transform.position) < _patrolRange)
        {
            Vector3 walkTo = new Vector3();
            for (int i = 0; i < 10; i++)
            {
                walkTo.x = Random.Range(-10, 10);
                walkTo.z = Random.Range(-10, 10);
                if (_patrolRange > Vector3.Distance(spawnPoint, transform.position + walkTo))//in range
                {
                    agent.SetDestination(transform.position + walkTo);
                }
            }
        }
        else
            agent.SetDestination(spawnPoint);
    }

    void ScanForTarget()
    {
        List<Collider> coliders = Physics.OverlapSphere(transform.position, _scanRadius, _attackable).ToList();
        if (coliders == null)
            return;
       
        int b = coliders.Count;
        for (int i = 0; i < b; i++)
        {
            if (!CheckIfInFront(coliders[i].transform.position)) // if not in front of player
            {
                coliders.Remove(coliders[i]);
                b--;
                i--;
            }
        }
        int a = coliders.Count;
        for (int i = 0; i < a; i++)
        {


            Collider c = GM.ClosestColliderInList(coliders);//if doesnt work, lets check if its the same object.

            RaycastHit hit;
            if (Physics.Raycast(transform.position, c.transform.position - transform.position, out hit, _scanRadius, _canSee))
            {
                if (hit.collider == c)
                {
                    Target = c.transform;
                    CurrentAlert = 0;
                    return;
                }
            }
            coliders.Remove(c);
        }
    }

    void FollowTaget()
    {
        agent.SetDestination(Target.position);
    }

    bool CheckIfInFront(Vector3 pos)
    {

        float targetAngle = Mathf.Atan2(transform.position.z- pos.z, transform.position.x - pos.x) * Mathf.Rad2Deg +90 ;
        float deltaAngleAIAndTarget = GM.AngleDifference(targetAngle, -transform.eulerAngles.y);
        //Debug.Log($"{targetAngle} , {-transform.eulerAngles.y} = {deltaAngleAIAndTarget}");
        if (deltaAngleAIAndTarget < _angleOfVision/2 && deltaAngleAIAndTarget > -_angleOfVision/2)
        {
            return true;
        }
            return false;
    }

    void AlertSystem() 
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Target.transform.position - transform.position, out hit, AlertRadius, _canSee))
        {
            if (hit.transform == Target)
            {
                AlertionRay(Color.blue);
                CurrentAlert += Time.deltaTime;
                CurrentAlert = CurrentAlert > MaxAlert ? MaxAlert : CurrentAlert;
                return;
            }
        }
        AlertionRay(Color.red);
        CurrentAlert -= Time.deltaTime;
        if (CurrentAlert < 0) { Target = null; }
    }

    void AlertionRay(Color c)
    {
        Debug.DrawRay(transform.position, Target.position - transform.position, c);
    }
    void DetectionRay()
    {
        Vector3 RightEye = new Vector3(Mathf.Sin(Mathf.Deg2Rad * _angleOfVision / 2), 0, Mathf.Cos(Mathf.Deg2Rad * _angleOfVision / 2));
        Vector3 LeftEye = new Vector3(Mathf.Sin(Mathf.Deg2Rad * -_angleOfVision / 2), 0, Mathf.Cos(Mathf.Deg2Rad * -_angleOfVision / 2));


        Debug.DrawRay(transform.position, transform.rotation * RightEye * _scanRadius, Color.green);
        Debug.DrawRay(transform.position, transform.rotation * LeftEye * _scanRadius, Color.green);
    }



}


