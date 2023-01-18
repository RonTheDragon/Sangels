﻿using UnityEngine;
public class AttackState : State
{
    public AttackState(StateManager stateManager, AIController aiController) : base(stateManager, aiController)
    {
        StateName = "Attack State";
    }

    public override void Enter()
    {
        AiController.AiAtackManager.AttackTarget();
    }

    public override void Exit()
    {
        Debug.Log("Exit Attack State");
    }

    public override void Update()
    {
        if (AiController.IsAlertAttack() && AiController.HasTarget())
        {
            AiController.FollowTarget();
            AiController.AiAtackManager.AttackTarget();
            
        }
        else if(AiController.HasTarget())
        {
            StateManager.ChangeState(new AlertState(StateManager, AiController));
        }
        else
            StateManager.ChangeState(new ScanState(StateManager, AiController));
    }
}