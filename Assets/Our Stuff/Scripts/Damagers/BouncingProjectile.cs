using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BouncingProjectile : ImpactProjectile
{
    Projectile fruit => GetComponent<Projectile>();
    [SerializeField] int _maxBouncing;
    [ReadOnly]public int BouncingCounter;//set to zero on lunch
    [SerializeField] float _maxBouncingDistance;
    [SerializeField] float _bouncePower;
    [ReadOnly] public List<GameObject> AlreadyHit;
    LayerMask BouncingFruitLayer => GameManager.instance.ProjectileBounceCanSee;


    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (Attackable == (Attackable | (1 << collision.gameObject.layer)))
            AlreadyHit.Add(collision.gameObject);

        if (BouncingCounter < _maxBouncing)//do the staff with the layers
        {
            Transform Target = FindBounceTarget();
            if (Target != null && AlreadyHit.FirstOrDefault(c => c.transform == Target) == null)
            {
                Debug.Log("Hited: " + AlreadyHit.FirstOrDefault(c => c.transform == Target));
                fruit.transform.LookAt(Target);
                fruit.LaunchProjectile(_bouncePower);
                BouncingCounter++;
            }
            
        }
        Debug.Log("has bounced");
    }



    Transform FindBounceTarget() 
    {
        Collider[] Colliders = Physics.OverlapSphere(transform.position, _maxBouncingDistance, Attackable);
        Collider target = null;
        float minDistance= _maxBouncingDistance;
        float tmpDistance;
        foreach (Collider collider in Colliders) 
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, collider.transform.position - transform.position, out hit, _maxBouncingDistance, BouncingFruitLayer)) 
            {
                tmpDistance = Vector3.Distance(transform.position, collider.transform.position);
                if (tmpDistance < minDistance)
                { 
                    minDistance = tmpDistance;
                    target=collider;
                }
            }
        }
        if(target==null)
            return null;
        else
            return target.transform;
    }




    
}
