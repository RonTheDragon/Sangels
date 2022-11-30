using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Health : MonoBehaviour
{
    //data
    public float MaxHealth;
    public float CurrentHealth;
    public bool _isDead;

    public abstract void TakeDamage(float damage, float knockback, Vector3 pushFrom);

    public abstract void TakeFire();

    public abstract void TakeStun();



}
