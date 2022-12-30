using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] PlayerCombatManager playerCombatManager;
    private void OnTriggerEnter(Collider other)
    {
        Icollectable Collectable = other.gameObject.GetComponent<Icollectable>();
        if (Collectable == null) return;

        if (Collectable is FruitCollectable)
        {
            FruitCollectable fruit = Collectable as FruitCollectable;
            if (playerCombatManager.CollectFruit(fruit.Fruit, fruit.Amount))
            {
                Collectable.PickUp();
            }
        }
    }

}
