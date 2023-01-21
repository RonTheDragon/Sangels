using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuredState : State
{
    public LuredState(StateManager stateManager, AIController aiController) : base(stateManager, aiController)
    {
        StateName = "Lured State";
    }

    public override void Enter()
    {
        AiController.LuredCooldown();
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        if (!AiController.HasTarget())
        {
            if (AiController.LuredTime <= 0)
            {
                StateManager.ChangeState(new RoamState(StateManager, AiController));
            }
            AiController.LookAtReset();
            AiController.DetectionRay();
            AiController.ScanCooldown();
            AiController.LuredCooldown();
        }
        else
        {
            StateManager.ChangeState(new AttackState(StateManager, AiController));
            // Debug.Log("change into attack state");
        }
    }

    
}
