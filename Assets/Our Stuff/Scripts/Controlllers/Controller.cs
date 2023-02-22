using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

abstract public class Controller : MonoBehaviour
{
    [Header("Walking")]
    [Tooltip("The movement speed")]
    public float Speed = 10;
    public float NormalSpeed = 10;
    public float RegularAnimationSpeed = 10;
    public float RotateSpeed = 50;

    [Header("Animation Rigging")]
    [SerializeField] private Transform _lookingAt;
    [SerializeField] private Rig _rig;
    [SerializeField] private float _lookingSpeed = 10;
    private float _targetWeight;

    //Force
    protected Vector3 _forceDirection;
    protected float _forceStrength;

    //Actions
    protected Action _loop;
    public Action OnStagger;
    public Action<float> OnStun;

    [Header("Glub")]
    [ReadOnly][SerializeField] protected float _glubCurrentEffect;
    [SerializeField] protected float _glubRemovedPS = 30;
    [SerializeField] protected float _glubMax = 100;

    //Refrences
    protected CharacterHealth _characterHealth =>GetComponent<CharacterHealth>();//remove the public later
    protected Animator _anim => transform.GetChild(0).GetComponent<Animator>();


    protected void Start()
    {
        _loop += ApplyingForce;
        _loop += MoveLookOverTime;
        _loop += GlubRemove;
    }
    protected void Update()
    {
        _loop?.Invoke();
    }

    #region Glub
    public void AddGlub(float glub)
    {
        _glubCurrentEffect += glub;
        if (_glubCurrentEffect > _glubMax)
            _glubCurrentEffect = _glubMax;
        SetSpeed();
    }

    private void GlubRemove()
    {
        if (_glubCurrentEffect > 0)
        {
            _glubCurrentEffect -= _glubRemovedPS * Time.deltaTime;

            if (_glubCurrentEffect > _glubMax)
                _glubCurrentEffect = _glubMax;
            SetSpeed();
        }
    }
    #endregion

    #region Force
    public void AddForce(Vector3 dir, float force)
    {
        _forceDirection = dir;
        _forceStrength = force;
    }

    /// <summary> Makes the added force move the player Overtime. </summary>
    protected abstract void ApplyingForce();
    #endregion

    #region Looking Animation
    public void LookAt(Vector3 pos)
    {
        _targetWeight = 1f;
        _lookingAt.transform.position = pos;
    }

    public void LookAtReset()
    {
        _targetWeight = 0f;
        _lookingAt.transform.position = transform.position + transform.forward;
    }

    private void MoveLookOverTime()
    {
        _rig.weight = Mathf.Lerp(_rig.weight, _targetWeight, _lookingSpeed * Time.deltaTime);
    }
    #endregion

    #region Speed get set
    public abstract float GetSpeed();

    public abstract void SetSpeed(float speed = -1);
    #endregion

    public virtual void Hurt(CharacterHealth.EffectFromImpactType impactType,float recievedStagger, float staggerResistance, GameObject attacker = null)
    {
        switch (impactType)
        {
            case CharacterHealth.EffectFromImpactType.Hurt:
                _anim.SetTrigger("Hurt"); _anim.SetFloat("Pain", recievedStagger/staggerResistance);
                break;

            case CharacterHealth.EffectFromImpactType.Stagger:
                OnStagger?.Invoke();
                _anim.SetBool("Stagger", true);
                break;

            case CharacterHealth.EffectFromImpactType.Stun:
                float StunTime = recievedStagger - (staggerResistance * 2);
                StunTime = StunTime / 10;
                OnStun?.Invoke(StunTime);
                _anim.SetTrigger("Stun");
                break;
        }
    }

    public bool CheckIfCanMove()
    {
        if (_characterHealth.IsDead ||
            _characterHealth.IsStunned ||
            _characterHealth.IsStaggered ||
            _characterHealth.IsGettingUp)
        {
            return false;
        }
        return true;
    }
}
