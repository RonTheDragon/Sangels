using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafCollectable : MonoBehaviour ,Icollectable
{
    public SOFruit.Fruit Fruit;
    public void PickUp()
    {
        gameObject.SetActive(false);
    }
}
