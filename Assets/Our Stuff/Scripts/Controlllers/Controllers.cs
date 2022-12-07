using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Controllers : MonoBehaviour
{
    [Header("Animation Rigging")]
    [SerializeField] Transform LookingAt;

    protected Vector3 _forceDirection;
    protected float _forceStrength;
    protected Action Loop;

    protected void Start()
    {
        Loop += applyingForce;
    }
    protected void Update()
    {
        Loop?.Invoke();
    }

    public void AddForce(Vector3 dir, float force)
    {
        _forceDirection = dir;
        _forceStrength = force;
    }

    /// <summary> Makes the added force move the player Overtime. </summary>
    protected abstract void applyingForce();

    public void LookAt(Vector3 pos)
    {
        LookingAt.transform.position = pos;
    }

    public void LookAtReset()
    {
        LookingAt.transform.position = transform.position + transform.forward;
    }
}
