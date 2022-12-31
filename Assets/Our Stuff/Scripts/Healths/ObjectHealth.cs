using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : Health
{
    [SerializeField] List<string> Drops;
    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger, GameObject Attacker = null)
    {
        CurrentHealth -= damage;
        if (CurrentHealth<0) { Die(); }
    }

    public override void Die()
    {
        foreach(string g in Drops)
        {
            ObjectPooler.Instance.SpawnFromPool(g, transform.position, transform.rotation);
        }
        gameObject.SetActive(false);
    }

}
