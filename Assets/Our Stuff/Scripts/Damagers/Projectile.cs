using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [Tooltip("Multiply The Speed")]
    [SerializeField] protected float ForceMultiplier = 1;
    protected Rigidbody RB => GetComponent<Rigidbody>();
    protected Collider c => GetComponent<Collider>();

    protected Transform SlingshotSit;

    public void LaunchProjectile(float Force)
    {
        transform.position += transform.forward * 0.1f;
        RB.velocity = Vector3.zero;
        SlingshotSit = null;
        RB.useGravity = true;
        c.enabled = true;
        transform.parent = ObjectPooler.Instance.transform;
        RB.AddForce(transform.forward * Force * ForceMultiplier,ForceMode.Impulse);
        RB.AddTorque(new Vector3(1, 1, 1) * Force);
    }

    public void SpawnOnSlingShot(Transform position)
    {
        SlingshotSit = position;
        transform.position -= transform.forward * 0.1f;
        RB.useGravity = false;
        RB.velocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero;
        c.enabled = false;
        transform.parent = SlingshotSit;
        BouncingProjectile bouncingProjectile = GetComponent<BouncingProjectile>();
        if (bouncingProjectile != null)
        {
            bouncingProjectile.BouncingCounter = 0;
            bouncingProjectile.AlreadyHit.Clear();
        }


    }

  
}
