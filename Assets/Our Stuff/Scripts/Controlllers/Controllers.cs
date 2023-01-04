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

    [Header("Glub")]
    [ReadOnly][SerializeField] protected float GlubCurrentEffect;
    [SerializeField] protected float GlubRemovedPS = 30;
    [SerializeField] protected float GlubMax = 100;
    CharacterHealth characterHealth =>GetComponent<CharacterHealth>();
    protected Animator anim => transform.GetChild(0).GetComponent<Animator>();

    float _targetWeight;

    protected void Start()
    {
        Loop += applyingForce;
        Loop += MoveLookOverTime;
        Loop += GlubRemove;
    }
    protected void Update()
    {
        Loop?.Invoke();
    }

    void GlubRemove()
    {
        if (GlubCurrentEffect > 0)
        {
            GlubCurrentEffect -= GlubRemovedPS * Time.deltaTime;

            if (GlubCurrentEffect > GlubMax)
                GlubCurrentEffect = GlubMax;
            SetSpeed();
        }
    }

    public void AddGlub(float glub)
    {
        GlubCurrentEffect += glub;
        if (GlubCurrentEffect > GlubMax)
            GlubCurrentEffect = GlubMax;
        SetSpeed();
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

    public abstract void SetSpeed(float speed = -1);

    public abstract float GetSpeed();



    public virtual void Hurt(float Pain, GameObject Attacker = null, bool Staggered = false)
    {
        if (Staggered && characterHealth.IsStagged) 
        {
            anim.SetTrigger("Stagger"); 
            OnStagger?.Invoke();
        }
        else
        {
            anim.SetTrigger("Hurt"); anim.SetFloat("Pain", Pain);
        }
    }
}
