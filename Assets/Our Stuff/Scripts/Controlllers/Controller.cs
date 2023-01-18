using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

abstract public class Controller : MonoBehaviour
{
    [Header("Walking")]
    [Tooltip("The movement speed")]
    [ReadOnly] public float Speed = 10;
    public float NormalSpeed = 10;
    public float RegularAnimationSpeed = 10;

    [Header("Animation Rigging")]

    [SerializeField] private Transform _lookingAt;
    [SerializeField] private Rig _rig;
    [SerializeField] private float _lookingSpeed = 10;

    protected Vector3 _forceDirection;
    protected float _forceStrength;
    protected Action _loop;

    public Action OnStagger;

    [Header("Glub")]
    [ReadOnly][SerializeField] protected float _glubCurrentEffect;
    [SerializeField] protected float _glubRemovedPS = 30;
    [SerializeField] protected float _glubMax = 100;
    public CharacterHealth _characterHealth =>GetComponent<CharacterHealth>();//remove the public later
    protected Animator _anim => transform.GetChild(0).GetComponent<Animator>();

    private float _targetWeight;

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

    public void AddGlub(float glub)
    {
        _glubCurrentEffect += glub;
        if (_glubCurrentEffect > _glubMax)
            _glubCurrentEffect = _glubMax;
        SetSpeed();
    }

    public void AddForce(Vector3 dir, float force)
    {
        _forceDirection = dir;
        _forceStrength = force;
    }

    /// <summary> Makes the added force move the player Overtime. </summary>
    protected abstract void ApplyingForce();

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

    public abstract float GetSpeed();

    public abstract void SetSpeed(float speed = -1);


    public virtual void Hurt(GameObject Attacker = null, float Staggered=1)
    {
        if (Staggered > 1 && Staggered < 2 && !_characterHealth.IsStaggered)
        {
            //Debug.Log("Stagger: " + Staggered);
            OnStagger?.Invoke();
            _anim.SetBool("Stagger", _characterHealth.IsStaggered);
            Debug.Log("testing");
        }
        else if (Staggered <= 1)
        {
            _anim.SetTrigger("Hurt"); _anim.SetFloat("Pain", Staggered);
            //Debug.Log("pain:"+ Pain);
        }
        else
        {
            _anim.SetTrigger("Stun");
        }

    }




    
}
