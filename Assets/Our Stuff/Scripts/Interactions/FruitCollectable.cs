using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;

public class FruitCollectable : MonoBehaviour, Icollectable
{
    public SOFruit.Fruit Fruit;
    public int Amount = 1;
    [HideInInspector] public bool Falling;

    Collider C => GetComponent<Collider>();
    Rigidbody RB => GetComponent<Rigidbody>();

    public void PickUp()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Falling)
        {
            C.isTrigger= false;
            RB.isKinematic = false;
            RB.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Falling = false;
        C.isTrigger = true;
        RB.isKinematic = true;
        RB.useGravity = false;
    }
}
