using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AiAtackManager : AttackManager
{

    [ReadOnly] public Transform Target;

    AIController aiController => GetComponentInParent<AIController>();


    void Awake()
    {
        Attackable = GM.EnemiesCanAttack;
        aiController.SetAnimator(anim);
    }

    public void AttackTarget()
    {
        OverrideToAttack();
        if (SOMeleeAttack.MinDist < Vector3.Distance(transform.position, Target.position) && Vector3.Distance(transform.position, Target.position) < SOMeleeAttack.MaxDist)
        {
            anim.SetTrigger(SOMeleeAttack.AnimationName);
            aiController.ChangeSpeed(SOMeleeAttack.speedWhileUsing);
            UsingAttackTimeLeft = SOMeleeAttack.UsingTime;
        }
    }
    

    protected override void AttackEnded()
    {
        aiController.ChangeSpeed(aiController.NormalSpeed);
    }
}

