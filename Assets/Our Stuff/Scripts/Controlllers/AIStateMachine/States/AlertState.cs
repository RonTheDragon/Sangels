using UnityEngine;
public class AlertState : State
{
    public AlertState(StateManager stateManager, AIController aiController) : base(stateManager,aiController)
    {
        StateName = "Alert State";
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
        Debug.Log("Exit Alert State");
    }

    public override void Update()
    {
        if (AiController.HasTarget() && AiController.IsAlertAttack())
        {
            StateManager.ChangeState(new AttackState(StateManager, AiController));
        }    
        else if(!AiController.HasTarget())
            StateManager.ChangeState(new ScanState(StateManager, AiController));
    }
}

