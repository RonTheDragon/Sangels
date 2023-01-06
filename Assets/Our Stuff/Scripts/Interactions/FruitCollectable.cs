using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;

public class FruitCollectable : MonoBehaviour, Icollectable
{
    public SOFruit.Fruit Fruit;
    public int Amount = 1;
    [HideInInspector] public bool Falling;

    private Collider _collider => GetComponent<Collider>();
    private Rigidbody _rigidBody => GetComponent<Rigidbody>();

    public void PickUp()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Falling)
        {
            _collider.isTrigger= false;
            _rigidBody.isKinematic = false;
            _rigidBody.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Falling = false;
        _collider.isTrigger = true;
        _rigidBody.isKinematic = true;
        _rigidBody.useGravity = false;
    }
}
