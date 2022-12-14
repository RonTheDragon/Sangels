using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatManager : MonoBehaviour
{
    [HideInInspector]
    public bool _melee;

    [SerializeField] protected float StaggeredTime = 2;

    [HideInInspector]
    public Action Loop;
    public Animator anim => GetComponent<Animator>();
    protected GameManager GM => GameManager.instance;

    protected float UsingAttackTimeLeft;

    [HideInInspector] public LayerMask Attackable;

    [HideInInspector] public List<Combat> Damagers = new List<Combat>();

    [SerializeField] protected SOMeleeAttack SOMeleeAttack;


    public void Update()
    {
        if (UsingAttackTimeLeft > 0)
        {
            UsingAttackTimeLeft -=Time.deltaTime;
        }
        else if (UsingAttackTimeLeft < 0) 
        {
            UsingAttackTimeLeft = 0f;
            AttackEnded();
            
        }
    }
    protected void OverrideToAttack()
    {
        foreach (Combat d in Damagers)
        {
            d.DamageAmount = SOMeleeAttack.DamageAmount;
            d.Knockback = SOMeleeAttack.Knockback;
            d.Stagger = SOMeleeAttack.Stagger;
        }

    }
    protected abstract void AttackEnded();

    protected virtual void Staggered()
    {
        UsingAttackTimeLeft = StaggeredTime;
    }

    public void JoinAttackerManager(Combat d)
    {
        Damagers.Add(d);
        d.Attackable = Attackable;
    }
}
