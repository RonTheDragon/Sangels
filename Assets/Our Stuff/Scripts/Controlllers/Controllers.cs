using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

abstract public class Controllers : MonoBehaviour
{
    [Header("Walking")]
    [Tooltip("The movement speed")]
    [ReadOnly] public float Speed = 10;
    public float NormalSpeed = 10;
    public float RegularAnimationSpeed = 10;

    [Header("Animation Rigging")]

    [SerializeField] Transform LookingAt;
    [SerializeField] Rig rig;
    [SerializeField] float _lookingSpeed = 10;

    protected Vector3 _forceDirection;
    protected float _forceStrength;
    protected Action Loop;

    public Action OnStagger;

    protected Animator anim => transform.GetChild(0).GetComponent<Animator>();

    float _targetWeight;

    protected void Start()
    {
        Loop += applyingForce;
        Loop += MoveLookOverTime;
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
        _targetWeight = 1f;
        LookingAt.transform.position = pos;
    }

    public void LookAtReset()
    {
        _targetWeight = 0f;
        LookingAt.transform.position = transform.position + transform.forward;
    }

    void MoveLookOverTime()
    {
        rig.weight = Mathf.Lerp(rig.weight, _targetWeight, _lookingSpeed * Time.deltaTime);
    }

    public abstract void ChangeSpeed(float speed = -1);


    public virtual void Hurt(float Pain, GameObject Attacker = null, bool Staggered = false)
    {
        if (Staggered && Speed!=0) 
        {
            anim.SetTrigger("Stagger"); OnStagger?.Invoke();
        }
        else
        {
            anim.SetTrigger("Hurt"); anim.SetFloat("Pain", Pain);
        }
    }
}
