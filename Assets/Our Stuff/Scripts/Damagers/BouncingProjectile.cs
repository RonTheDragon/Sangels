using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BouncingProjectile : ImpactProjectile
{
    [ReadOnly] public int BouncingCounter;//set to zero on lunch
    [ReadOnly] public List<GameObject> AlreadyHit;
    private Projectile _fruit => GetComponent<Projectile>();    
    [SerializeField] private int _maxBouncing;
    [SerializeField] private float _maxBouncingDistance;
    [SerializeField] private float _bouncePower;
    [SerializeField] private float _minimumVel=1;
    private LayerMask _bouncingFruitLayer => GameManager.Instance.ProjectileBounceCanSee;


    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        
        if (Attackable == (Attackable | (1 << collision.gameObject.layer)))
            AlreadyHit.Add(collision.gameObject);

        if (BouncingCounter < _maxBouncing && _rigidBody.velocity.magnitude > _minimumVel)//do the staff with the layers
        {
            Transform Target = FindBounceTarget();
                if (Target != null && AlreadyHit.FirstOrDefault(c => c.transform == Target) == null)
                {
                    Debug.Log("bounced");
                    _fruit.transform.LookAt(Target.position);
                    _fruit.LaunchProjectile(_rigidBody.velocity.magnitude * _bouncePower);
                    BouncingCounter++;
                }
        }  
        
       // Debug.Log("has bounced");
    }



    private Transform FindBounceTarget() 
    {
        Collider[] Colliders = Physics.OverlapSphere(transform.position, GetMaxBouncingDistance(), Attackable);
        Collider target = null;
        float minDistance= GetMaxBouncingDistance();
        float tmpDistance;
        foreach (Collider collider in Colliders) 
        {
            
            RaycastHit hit;
            if (Physics.Raycast(transform.position, collider.transform.position - transform.position, out hit, GetMaxBouncingDistance(), _bouncingFruitLayer)) 
            {
                if (hit.collider == collider)
                {
                    tmpDistance = Vector3.Distance(transform.position, collider.transform.position);
                    if (tmpDistance < minDistance)
                    {
                        minDistance = tmpDistance;
                        target = collider;                 
                    }
                }
            }
        }
        if(target==null)
            return null;
        else
            return target.transform;
    }

    private float GetMaxBouncingDistance() 
    {
        return _rigidBody.velocity.magnitude* _maxBouncingDistance;
    }





    
}
