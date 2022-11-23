using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [Tooltip("x = Min Cooldown\n y = Max Cooldown\n and the random picks between them")]
    [SerializeField] Vector2 _roamCooldown = new Vector2(5, 10);
    NavMeshAgent agent => GetComponent<NavMeshAgent>();
    [SerializeField] float _patrolRange = 20;
    Vector3 spawnPoint=> transform.position;

    [ReadOnly][SerializeField] Transform Target;
    //stored data
    float _roamCD;
    float _scanCD;
    [SerializeField] float _scanRadius;
    [SerializeField] LayerMask _canSee;
    [SerializeField] LayerMask _attackable; 
    //List<Collider> _colidersList = new List<Collider>();   
    //void Start()
    //{
    //    
    //}

    void Update()
    {
        AIbrain();
        RayCastDoSomething();
        if (Target != null)
            Debug.DrawRay(transform.position, Target.position-transform.position, Color.blue);
    }

    void AIbrain()
    {
        if (Target)
        {
            FollowTaget();
        }
        else
        {
            RoamCooldown();
            ScanCooldown();
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
                _scanCD -= Time.deltaTime;
            }
    }

    void ScanCooldown()
    {

        if (_scanCD <= 0)
        {
            ScanForTarget();
            _scanCD = 1;
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


            Collider c = ClosestTarget(coliders);//if doesnt work, lets check if its the same object.

            RaycastHit hit;
            if (Physics.Raycast(transform.position, c.transform.position - transform.position, out hit, _scanRadius, _canSee))
            {
                if (hit.collider == c)
                {
                    Target = c.transform;
                    return;
                }
            }
            coliders.Remove(c);
        }
    }

    public Collider ClosestTarget( List<Collider> coliders) 
    {
        if (coliders== null)
            return null;
        if(coliders.Count==1)
            return coliders.FirstOrDefault();
        float MinDist = Mathf.Infinity;
        int ClosestColliderIndex = 0;
        for (int i = 0; i < coliders.Count -1; i++)
        {
            float dist =Vector3.Distance(coliders[i].transform.position, transform.position);
            if (MinDist > dist)
            { 
                MinDist = dist;
                ClosestColliderIndex = i;
            }
        }
        return coliders[ClosestColliderIndex];
    }

    void FollowTaget()
    {
        agent.SetDestination(Target.position);
    }

    bool CheckIfInFront(Vector3 pos)
    {
        float targetAngle = Mathf.Atan2(transform.position.z- pos.z, transform.position.x - pos.x) * Mathf.Rad2Deg ;
        float deltaAngleAIAndTarget = targetAngle - transform.localEulerAngles.y;
        Debug.Log($"{targetAngle} - {transform.eulerAngles.y} = {deltaAngleAIAndTarget}");
        if (deltaAngleAIAndTarget < 45 && deltaAngleAIAndTarget > -45)
        {
            return true;
        }
            return false;
    }

    void RayCastDoSomething() 
    {
        
        
    }



}


