using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : ProjectileDamage
{
    [SerializeField] List<ParticleSystem> ExplosionParticles;
    public float Radius;

    public void Explode()
    {
        foreach(ParticleSystem p in ExplosionParticles)
        {
            p.Play();
        }

        Collider[] cols = Physics.OverlapSphere(transform.position, Radius);
        foreach (Collider c in cols)
        {
            if (Attackable == (Attackable | (1 << c.gameObject.layer)))
            {
                Health hp = c.transform.GetComponent<Health>();
                if (hp != null)
                {
                    Vector3 pos = c.ClosestPointOnBounds(transform.position);
                    float dist = Vector3.Distance(transform.position, pos);
                    float DistanceMultipler = (-dist / Radius) + 1;
                    hp.TakeDamage(DamageAmount * DistanceMultipler, Knockback * DistanceMultipler, transform.position, Stagger * DistanceMultipler, Shooter);
                }
            }

        }
        
    }
}
