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

    List<Collider> colliders;
    //List<Collider> _colidersList = new List<Collider>();   
    //void Start()
    //{
    //    
    //}

    // Update is called once per frame
    void Update()
    {
        AIbrain();
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
        colliders = Physics.OverlapSphere(transform.position, _scanRadius).ToList();
        foreach (Collider collider in colliders)
        {
            if (!CheckIfInFront(collider.transform.position)) // if not in front of player
            {
                colliders.Remove(collider);
            }
        }
    }

    void FollowTaget()
    {
        agent.SetDestination(Target.position);
    }

    bool CheckIfInFront(Vector3 pos)
    {
        float targetAngle = Mathf.Atan2(transform.position.z-Target.position.z, transform.position.x - Target.position.x) * Mathf.Rad2Deg ;
        float deltaAngleAIAndTarget = targetAngle - transform.eulerAngles.y;
        if (deltaAngleAIAndTarget < 45 && deltaAngleAIAndTarget > -45)
        {
            return true;
        }
        return false;
    }
}


