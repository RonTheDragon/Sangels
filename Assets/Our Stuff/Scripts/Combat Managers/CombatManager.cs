using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatManager : MonoBehaviour
{
    [HideInInspector]
    public bool IsMelee;

    [SerializeField] protected float _staggeredTime = 1.3f;

    [HideInInspector]
    public Action Loop;
    public Animator Anim => GetComponent<Animator>();
    protected GameManager _gm => GameManager.Instance;

    protected float _usingAttackTimeLeft;

    [HideInInspector] public LayerMask Attackable;

    [HideInInspector] public List<Combat> Damagers = new List<Combat>();

    public SOMeleeAttack SOMeleeAttack;


    public void Update()
    {
        if (_usingAttackTimeLeft > 0)
        {
            _usingAttackTimeLeft -=Time.deltaTime;
        }
        else if (_usingAttackTimeLeft < 0) 
        {
            _usingAttackTimeLeft = 0f;
            AttackEnded();
            
        }
    }

    protected abstract void AttackEnded();

    protected virtual void Staggered()
    {
        _usingAttackTimeLeft = _staggeredTime;
    }
}
