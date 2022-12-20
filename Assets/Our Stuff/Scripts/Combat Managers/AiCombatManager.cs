using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AiCombatManager : CombatManager
{

    [ReadOnly] public Transform Target;

    AIController aiController => GetComponentInParent<AIController>();

    private void Start()
    {
        aiController.OnStagger += Staggered;
        Attackable = GM.EnemiesCanAttack;
    }

    public void AttackTarget()
    {
        if (Target == null || UsingAttackTimeLeft>0) return;
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

    protected override void Staggered()
    {
        base.Staggered();
        aiController.ChangeSpeed(0);
    }
}
