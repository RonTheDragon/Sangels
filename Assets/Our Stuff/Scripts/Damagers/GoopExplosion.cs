using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoopExplosion : Explosion
{
    [SerializeField] private GameObject _goop;
    private Rigidbody _rigidBody => GetComponent<Rigidbody>();
    private PublicCollisions _collisions => _goop.GetComponent<PublicCollisions>();
    private bool _gooped;

    private LayerMask _goopMask => GameManager.Instance.GoopStick;


    private void Start()
    {
        _collisions.OnTriggerStayEvent += OnTriggerStayEvent;
    }

    private void OnTriggerStayEvent(Collider other)
    {
        if (_gooped)
        {     
                if (Attackable == (Attackable | (1 << other.gameObject.layer)))
                {
                    Controller control = other.GetComponent<Controller>();
                     if (control != null)
                     {
                         control.AddGlub(100);
                     }
                }
        }
    }

    public override void Explode()
    {
        base.Explode();
        _rigidBody.isKinematic = false;
        _goop.SetActive(false);
        _gooped = false;
        _rigidBody.velocity = Vector3.down * 3;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_gooped && _goopMask == (_goopMask | (1 << collision.gameObject.layer)))
        {
            _gooped = true;
            Vector3 normal = collision.contacts[0].normal;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.down, -normal);
            transform.rotation = rotation;
            _goop.SetActive(true);
            _rigidBody.isKinematic = true;
        }
    }


}
