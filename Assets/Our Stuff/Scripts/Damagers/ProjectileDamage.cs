using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : Damage
{
    [HideInInspector] public GameObject Shooter;
    [Tooltip("x = Min Speed, slower wont cause damage\ny = Max Speed, On This Speed The Fruit Causes his full damage")]
    [SerializeField] Vector2 DamageBySpeed;
    Rigidbody rb => GetComponent<Rigidbody>();

    

    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        float speed = rb.velocity.magnitude;

        if (Attackable == (Attackable | (1 << collision.gameObject.layer)) && speed > DamageBySpeed.x)
        {
            Health hp = collision.gameObject.GetComponent<Health>();
            if (hp!=null)
            {
                if (speed > DamageBySpeed.y) speed= DamageBySpeed.y; 
                float SpeedMutiply = speed /DamageBySpeed.y;

                hp.TakeDamage(DamageAmount * SpeedMutiply, Knockback * SpeedMutiply, transform.position, Stagger*SpeedMutiply, Shooter);
            }
        }
    }
}
