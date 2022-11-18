using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawmIfFallsOffMap : MonoBehaviour
{
    [HideInInspector]
    public Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (transform.position.y < -10)
        {
            CharacterController CC = transform.GetComponent<CharacterController>();
            Rigidbody rb = transform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }
            if (CC != null)
            {
                CC.enabled = false;
                transform.position = startPos;
                CC.enabled = true;
            }
            else transform.position = startPos;

        }
    }
}