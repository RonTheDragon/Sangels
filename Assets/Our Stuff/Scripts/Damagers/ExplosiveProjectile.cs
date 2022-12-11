using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : ProjectileDamage
{
    [SerializeField] string Explosion;
    [SerializeField] float Radius = 10;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != Shooter)
        {
            Explosion explosion = ObjectPooler.Instance.SpawnFromPool(Explosion, transform.position, transform.rotation).GetComponent<Explosion>();
            explosion.DamageAmount = DamageAmount;
            explosion.Knockback= Knockback; 
            explosion.Stagger=Stagger;
            explosion.Radius=Radius;
            explosion.Shooter=Shooter;
            explosion.Attackable=Attackable; 
            explosion.Explode();
            gameObject.SetActive(false);
        }
    }
}
