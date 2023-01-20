using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


public class AiCombatManager : CombatManager
{
    private AIController _aiController => GetComponentInParent<AIController>();

    new protected void Start()
    {
        base.Start();

        Attackable = _gm.EnemiesCanAttack;

        _aiController.OnStagger += Staggered;
        _aiController.OnStun += Stunned;
    }

    /// <summary> Caculating if the target can be attacked </summary>
    public void TryAttackTarget()
    {
        if (_aiController.Target == null || CheckIfBusy())  return;  //  Cancel if Missing Target OR on Cooldown
        float dist = Vector3.Distance(transform.position, _aiController.Target.position); // if in Distance
        if (SOMeleeAttack.MinDist < dist && SOMeleeAttack.MaxDist > dist)
        {
            AttackTarget();
        }
    }

    /// <summary> Launching an attack </summary>
    protected void AttackTarget()
    {
        _anim.SetTrigger(SOMeleeAttack.AnimationName);
        _aiController.SetSpeed(SOMeleeAttack.SpeedWhileUsing);
        _busyTimeLeft = SOMeleeAttack.UsingTime;
    }
    
    /// <summary>
    /// Ending The Attack
    /// </summary>
    protected override void BusyEnded()
    {
        base.BusyEnded();
        _aiController.SetSpeed(_aiController.NormalSpeed);
    }

    #region AttackImpacts

    protected override void Staggered()
    {
        base.Staggered();
        _aiController.StopMoving();
    }

    protected override void Stunned(float StunTime)
    {
        base.Stunned(StunTime);
        _aiController.LookAtReset();
        _aiController.StopMoving();
    }

    protected override void EndStunned()
    {
        StandingUp();
    }

    #endregion
}

