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
        RB.velocity = Vector3.zero;
        SlingshotSit = null;
        RB.useGravity = true;
        c.enabled = true;
        transform.parent = ObjectPooler.Instance.transform;
        RB.AddForce(transform.forward * Force * ForceMultiplier);
        RB.AddTorque(new Vector3(1, 1, 1) * Force);
    }

    public void SpawnOnSlingShot(Transform position)
    {
        SlingshotSit = position;
        RB.useGravity = false;
        RB.velocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero;
        c.enabled = false;
        transform.parent = SlingshotSit;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
