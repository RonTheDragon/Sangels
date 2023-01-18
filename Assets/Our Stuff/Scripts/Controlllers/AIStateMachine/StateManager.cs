using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    AIController _aiController => GetComponent<AIController>();
    State _currentState;
    [ReadOnly][SerializeField] string _currentStateName;

    void Start()
    {
        _currentState = new RoamState(this, _aiController);
        _currentStateName = _currentState.StateName;
    }

    public void ChangeState(State newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }

        _currentState = newState;
        _currentStateName=newState.StateName;

        if (_currentState != null)
        {
            _currentState.Enter();
        }
    }

    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.Update();
        }
        else
            Debug.Log("there is no current state");
    }
}
