using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Explosion : ProjectileDamage
{
    public float Radius;

    [SerializeField] private List<ParticleSystem> _explosionParticles;
    [SerializeField] private float _timeTillTurnOff;

    public virtual void Explode()
    {
        foreach(ParticleSystem p in _explosionParticles)
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
                    if (hp is CharacterHealth)
                    {
                        CharacterHealth characterHealth = (CharacterHealth)hp;
                        if (Fire > 0) characterHealth.TakeFire(Fire * DistanceMultipler);
                    }
                }
            }

        }
    }
}
