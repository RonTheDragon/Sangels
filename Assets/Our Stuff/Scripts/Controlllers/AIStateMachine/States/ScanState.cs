using UnityEngine;
public class ScanState : State
{
    public ScanState(StateManager stateManager, AIController aiController) : base(stateManager, aiController)
    {
        StateName = "Scan State";
    }

    public override void Enter()
    {
        AiController.ScanForTarget();
    }

    public override void Exit()
    {
        Debug.Log("exit Scan State");
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
            StateManager.ChangeState(new AlertState(StateManager, AiController));
            //Debug.Log("change into Alert State");
        }
    }



}
