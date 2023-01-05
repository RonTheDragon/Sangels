using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactProjectile : ProjectileDamage
{
    [Tooltip("x = Min Speed, slower wont cause damage\ny = Max Speed, On This Speed The Fruit Causes his full damage")]
    [SerializeField] private Vector2 EffectBySpeed;
    protected Rigidbody _rigidBody => GetComponent<Rigidbody>();

    


    protected virtual void OnCollisionEnter(Collision collision)
    {
        float speed = _rigidBody.velocity.magnitude;

        if (Attackable == (Attackable | (1 << collision.gameObject.layer)) && speed > EffectBySpeed.x)
        {
            Health hp = collision.gameObject.GetComponent<Health>();
            if (hp!=null)
            {
                if (speed > EffectBySpeed.y) speed= EffectBySpeed.y; 
                float SpeedMutiply = speed /EffectBySpeed.y;

                hp.TakeDamage(DamageAmount * SpeedMutiply, Knockback * SpeedMutiply, transform.position, Stagger*SpeedMutiply, Shooter);
            }
        }
    }
}
