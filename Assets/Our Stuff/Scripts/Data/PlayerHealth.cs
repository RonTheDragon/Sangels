using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom)
    {
        throw new System.NotImplementedException();
    }
    public override void TakeFire()
    {
        throw new NotImplementedException();
    }
    public override void TakeStun()
    {
        throw new NotImplementedException();
    }
}
