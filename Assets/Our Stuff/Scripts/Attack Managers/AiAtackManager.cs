using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AiAtackManager : AttackManager
{

    [ReadOnly] public Transform Target;
    [SerializeField] SOMeleeAttack SOMeleeAttack;

    public void AttackTarget()
    {
        OverrideToAttack();


    }
    void OverrideToAttack()
    {
        if (SOMeleeAttack.MinDist < Vector3.Distance(transform.position, Target.position) && Vector3.Distance(transform.position, Target.position) < SOMeleeAttack.MaxDist)
            anim.SetTrigger(SOMeleeAttack.AnimationName);
    }
}

