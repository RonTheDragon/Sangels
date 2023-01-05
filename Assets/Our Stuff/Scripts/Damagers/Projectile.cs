using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [Tooltip("Multiply The Speed")]
    public float ForceMultiplier = 1;
    protected Rigidbody _rigidBody => GetComponent<Rigidbody>();
    protected Collider _collider => GetComponent<Collider>();

    protected Transform _slingshotSit;

    public void LaunchProjectile(float Force)
    {
        transform.position += transform.forward * 0.1f;
        _rigidBody.velocity = Vector3.zero;
        _slingshotSit = null;
        _rigidBody.useGravity = true;
        _collider.enabled = true;
        transform.parent = ObjectPooler.Instance.transform;
        _rigidBody.AddForce(transform.forward * Force * ForceMultiplier,ForceMode.Impulse);
        _rigidBody.AddTorque(new Vector3(1, 1, 1) * Force);
    }

    public void SpawnOnSlingshot(Transform position)
    {
        _slingshotSit = position;
        transform.position -= transform.forward * 0.1f;
        _rigidBody.useGravity = false;
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
        _collider.enabled = false;
        transform.parent = _slingshotSit;
        BouncingProjectile bouncingProjectile = GetComponent<BouncingProjectile>();
        if (bouncingProjectile != null)
        {
            bouncingProjectile.BouncingCounter = 0;
            bouncingProjectile.AlreadyHit.Clear();
        }


    }

  
}
