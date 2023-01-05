using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : ProjectileDamage
{
    [SerializeField] private string _explosion;
    [SerializeField] protected float _radius = 10;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != Shooter)
        {
            Explosion explosion = ObjectPooler.Instance.SpawnFromPool(_explosion, transform.position, transform.rotation).GetComponent<Explosion>();
            explosion.DamageAmount = DamageAmount;
            explosion.Knockback= Knockback; 
            explosion.Stagger=Stagger;
            explosion.Radius=_radius;
            explosion.Shooter=Shooter;
            explosion.Fire = Fire;
            explosion.Attackable=Attackable; 
            explosion.Explode();
            gameObject.SetActive(false);
        }
    }
}
