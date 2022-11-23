using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [Tooltip("x = Min Cooldown\n y = Max Cooldown\n and the random picks between them")]
    [SerializeField] Vector2 _roamCooldown = new Vector2(5, 10);
    NavMeshAgent agent => GetComponent<NavMeshAgent>();

    //stored data
    float _roamCD;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
        Vector3 walkTo = new Vector3();
        walkTo.x = Random.Range(-10, 10);
        walkTo.z = Random.Range(-10, 10);
        agent.SetDestination(transform.position+walkTo);
    }
}


