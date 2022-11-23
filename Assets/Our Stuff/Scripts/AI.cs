using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [Tooltip("x = Min Cooldown\n y = Max Cooldown\n and the random picks between them")]
    [SerializeField] Vector2 _roamCooldown = new Vector2(5, 10);
    NavMeshAgent agent => GetComponent<NavMeshAgent>();
    [SerializeField] float _patrolRange = 20;
    Vector3 spawnPoint=> transform.position;
    //stored data
    float _roamCD;
    
    
    //void Start()
    //{
    //    
    //}

    // Update is called once per frame
    void Update()
    {
        RoamCooldown();
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
}


