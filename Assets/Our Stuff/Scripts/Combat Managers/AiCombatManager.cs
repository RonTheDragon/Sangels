using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AiCombatManager : CombatManager
{

    [ReadOnly] public Transform Target;

    private AIController _aiController => GetComponentInParent<AIController>();

    private void Start()
    {
        _aiController.OnStagger += Staggered;
        Attackable = _gm.EnemiesCanAttack;
    }

    public void AttackTarget()
    {
        if (Target == null || _usingAttackTimeLeft>0) return;
        if (SOMeleeAttack.MinDist < Vector3.Distance(transform.position, Target.position) && Vector3.Distance(transform.position, Target.position) < SOMeleeAttack.MaxDist)
        {
            Anim.SetTrigger(SOMeleeAttack.AnimationName);
            _aiController.SetSpeed(SOMeleeAttack.SpeedWhileUsing);
            _usingAttackTimeLeft = SOMeleeAttack.UsingTime;
        }
    }
    

    protected override void AttackEnded()
    {
        _aiController.SetSpeed(_aiController.NormalSpeed);
    }

    protected override void Staggered()
    {
        base.Staggered();
        _aiController.SetSpeed(0);
    }
}

