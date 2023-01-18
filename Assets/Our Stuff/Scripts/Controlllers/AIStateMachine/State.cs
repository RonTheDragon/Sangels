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


    public abstract void Enter(); // does the first things when entering the state like setting bool of animator to true
    public abstract void Exit(); // set things at the end of the state like setting bool of animator to false
    public abstract void Update(); // called in the update event of the state machine and does what should be in update





}
