using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatManager : MonoBehaviour
{
    [HideInInspector]
    public bool IsMelee;

    [SerializeField] protected float _staggeredTime = 1.3f;
    [SerializeField] protected float _minimumStunTime = 1.3f;
    [SerializeField] protected float _standUpTime = 3f;

    [HideInInspector]
    public Action Loop;
    protected Animator _anim => GetComponent<Animator>();
    protected GameManager _gm => GameManager.Instance;
    protected CharacterHealth _health => GetComponentInParent<CharacterHealth>();

    protected float _busyTimeLeft;

    [HideInInspector] public LayerMask Attackable;

    [HideInInspector] public List<Combat> Damagers = new List<Combat>();

    public SOMeleeAttack SOMeleeAttack;


    protected void Start()
    {
        Loop += BusyCooldown;
    }

    protected void Update()
    {
        Loop?.Invoke();
    }

    private void BusyCooldown()
    {
        if (_busyTimeLeft > 0)
        {
            _busyTimeLeft -= Time.deltaTime;
        }
        else if (_busyTimeLeft < 0)
        {
            _busyTimeLeft = 0f;
            BusyEnded();
        }
    }

    protected virtual void BusyEnded()
    {
        if (_health.IsStunned)
        {
            EndStunned();
        }
        else if (_health.IsStaggered)
        {
            EndStaggered();
        }
        else if (_health.IsGettingUp)
        {
            EndStandingUp();
        }
    }

    #region AttackImpacts
    protected virtual void Staggered()
    {
        _busyTimeLeft = _staggeredTime;
        _anim.SetTrigger("Stagger");
        _anim.SetBool("Stop", true);
    }

    protected virtual void Stunned(float StunTime)
    {
        _busyTimeLeft = _minimumStunTime + StunTime;
        _anim.SetTrigger("Stun");
        _anim.SetBool("Stop", true);
    }

    protected virtual void StandingUp()
    {
        _busyTimeLeft = _standUpTime;
        _anim.SetTrigger("StandUp");
        _health.IsStunned = false;
        _health.IsGettingUp = true;
        _anim.SetBool("Stop", false);
    }

    protected virtual void EndStaggered()
    {
        _health.IsStaggered = false;
        _anim.SetBool("Stop", false);
    }

    protected abstract void EndStunned();

    protected virtual void EndStandingUp() 
    {
        _health.IsGettingUp = false;
    }
    #endregion

    public virtual bool CheckIfBusy()
    {
        if (_health.IsDead ||
            _health.IsStunned ||
            _health.IsStaggered ||
            _health.IsGettingUp ||
            _busyTimeLeft > 0)
        {
            return true;
        }
        return false;
    }
}
