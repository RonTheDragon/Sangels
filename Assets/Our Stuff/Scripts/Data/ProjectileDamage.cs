using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : Damage
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Attackable == (Attackable | (1 << collision.gameObject.layer)))
        {
            Health hp = collision.gameObject.GetComponent<Health>();
            if (hp!=null)
            {
                
            }
        }
    }
}
