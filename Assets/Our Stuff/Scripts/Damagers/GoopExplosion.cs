using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoopExplosion : Explosion
{
    [SerializeField] GameObject Goop;
    Rigidbody RB => GetComponent<Rigidbody>();
    bool gooped;

    [SerializeField] float _effectCooldown = 0.5f;
    float _effectCool;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gooped && RB.velocity.magnitude < 0.1f) { gooped = true; Goop.SetActive(true); transform.rotation = Quaternion.identity; RB.isKinematic = true; }

        if (gooped)
        {
            if (_effectCool > 0)
            {
                _effectCool -= Time.deltaTime;
            }
            else
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);
                foreach(Collider collider in colliders)
                {
                    if (Attackable == (Attackable | (1 << collider.gameObject.layer)))
                    {
                        Controllers control = collider.GetComponent<Controllers>();
                        control.AddGlub(100);
                    }
                }
                _effectCool = _effectCooldown;
            }
        }
    }

    public override void Explode()
    {
        base.Explode();
        RB.isKinematic = false;
        Goop.SetActive(false);
        gooped = false;
        RB.velocity = Vector3.down*3;
    }
}
