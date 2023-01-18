using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public AIController AiController { get; protected set;}
    public StateManager StateManager { get; protected set;}
    public string StateName { get; protected set;}
    public State(StateManager stateManager, AIController aiController)
    {
        StateManager = stateManager;
        AiController = aiController;
    }


    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();





}
