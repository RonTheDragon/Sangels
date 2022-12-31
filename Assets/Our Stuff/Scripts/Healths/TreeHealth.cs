using System.Collections;
using UnityEngine;

public class TreeHealth : Health
{
    FruitTree tree => transform.parent.GetComponent<FruitTree>();
    new void Start()
    {
        base.Start();
        StartCoroutine("WaitOneFrame");
    }
    IEnumerator WaitOneFrame()
    {
        yield return null;
        tree.GrowFruits();
    }

    private void Update()
    {
        if (_isDead)
        {
            CurrentHealth += Time.deltaTime / tree.CurrentRegrowTime * MaxHealth;
        }
        if (CurrentHealth > MaxHealth)
        {
            _isDead = false;
            CurrentHealth = MaxHealth;
            tree.GrowFruits();
        }
    }

    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger, GameObject Attacker = null)
    {
        if (!_isDead)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0) { Die(); }
        }
    }

    public override void Die()
    {
        tree.DropFruits();
        tree.RandomizeGrowTime();
        _isDead = true;
    }



    
}
