using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;

public class FruitCollectable : MonoBehaviour, Icollectable
{
    public SOFruit.Fruit Fruit;
    public int Amount = 1;

    public void PickUp()
    {
        gameObject.SetActive(false);
    }
}
