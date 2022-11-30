using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
public abstract class Health : MonoBehaviour
{
    //data
    public float MaxHealth;
    [ReadOnly][SerializeField] float CurrentHealth;
    private bool _isDead;


    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public abstract void TakeDamage(float damage, float knockback, Vector3 pushFrom);

    public abstract void TakeFire();

    public abstract void TakeStun();



}
