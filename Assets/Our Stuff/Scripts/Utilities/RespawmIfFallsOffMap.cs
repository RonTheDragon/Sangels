using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawmIfFallsOffMap : MonoBehaviour
{
    [HideInInspector]
    public Vector3 StartPos;
    private GameManager _gm => GameManager.Instance;
    private float _limit;

    private void Start()
    {
        StartPos = transform.position;
        _limit = _gm.TheLevelManager.RespawnIfFallBelowY;
    }

    private void Update()
    {
        if (transform.position.y < _limit)
        {
            CharacterController CC = transform.GetComponent<CharacterController>();
            Rigidbody rb = transform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            if (CC != null)
            {
                CC.enabled = false;
                transform.position = StartPos;
                CC.enabled = true;
            }
            else transform.position = StartPos;

        }
    }
}