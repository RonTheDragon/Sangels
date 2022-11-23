using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour,IpooledObject
{
    [Tooltip("Multiply The Speed")]
    [SerializeField] protected float ForceMultiplier = 1;
    protected Rigidbody RB => GetComponent<Rigidbody>();

    protected Transform SlingshotSit;
    public void OnObjectSpawn()
    {
        RB.velocity = Vector3.zero;
    }

    public void LaunchProjectile(float Force)
    {
        RB.velocity = Vector3.zero;
        SlingshotSit = null;
        RB.useGravity = true;
        transform.parent = ObjectPooler.Instance.transform;
        RB.AddForce(transform.forward * Force * ForceMultiplier);
    }

    public void SpawnOnSlingShot(Transform position)
    {
        SlingshotSit = position;
        RB.useGravity = false;
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
