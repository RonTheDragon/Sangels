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
        if (IsDead)
        {
            CurrentHealth += Time.deltaTime / tree.CurrentRegrowTime * MaxHealth;
        }
        if (CurrentHealth > MaxHealth)
        {
            IsDead = false;
            CurrentHealth = MaxHealth;
            tree.GrowFruits();
        }
    }

    public override void TakeDamage(float damage, float knockback, Vector3 pushFrom, Vector2 Stagger, GameObject Attacker = null)
    {
        if (!IsDead)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0) { Die(); }
        }
    }

    public override void Die()
    {
        tree.DropFruits();
        tree.RandomizeGrowTime();
        IsDead = true;
    }



    
}
