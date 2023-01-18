using UnityEngine;
public class RoamState : State
{
    public RoamState(StateManager stateManager, AIController aiController) : base(stateManager, aiController)
    {
        StateName = "Roam State";
    }

    public override void Enter()
    {
        AiController.WalkAround();
    }

    public override void Exit()
    {
        Debug.Log("exit Roam State");
    }

    public override void Update()
    {
        if (!AiController.HasTarget())
        {
            AiController.LookAtReset();
            AiController.DetectionRay();
            AiController.ScanCooldown();
            AiController.RoamCooldown();
        }
        else
        { 
            StateManager.ChangeState(new AttackState(StateManager, AiController));
            Debug.Log("change into attack state");
        }
    }
}