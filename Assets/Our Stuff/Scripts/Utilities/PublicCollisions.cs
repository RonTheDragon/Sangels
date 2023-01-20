using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This Script is Made so we can Get Collisions and Handle them from Somewhere else
public class PublicCollisions : MonoBehaviour
{
    public Action<Collider> OnTriggerStayEvent;
    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayEvent?.Invoke(other);
    }
}
