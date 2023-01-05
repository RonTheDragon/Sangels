using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoopExplosion : Explosion
{
    [SerializeField] private GameObject _goop;
    private Rigidbody _rigidBody => GetComponent<Rigidbody>();
    private bool _gooped;

    [SerializeField] private float _effectCooldown = 0.5f;
    private float _effectCool;

    private LayerMask _goopMask => GameManager.Instance.GoopStick;

 

    // Update is called once per frame
    private void Update()
    {
        // if (!gooped && RB.velocity.magnitude < 0.1f) { gooped = true; Goop.SetActive(true); transform.rotation = Quaternion.identity; RB.isKinematic = true; }

        if (_gooped)
        {
            if (_effectCool > 0)
            {
                _effectCool -= Time.deltaTime;
            }
            else
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);
                foreach (Collider collider in colliders)
                {
                    if (Attackable == (Attackable | (1 << collider.gameObject.layer)))
                    {
                        Controller control = collider.GetComponent<Controller>();
                        if (control != null)
                        {
                            control.AddGlub(100);
                        }
                    }
                }
                _effectCool = _effectCooldown;
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
