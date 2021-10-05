using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    private State _currentState;
    private List<State> _allStates;

    public void AddState(State state)
    {
        if (_currentState == null)
        {
            _allStates = new List<State>();
            _currentState = state;
        }



        if (_allStates.Contains(state)) return;

        _allStates.Add(state);
    }

    public void SetState<T>() where T : State
    {
        for (int i = 0; i < _allStates.Count; i++)
        {
            if (_allStates[i].GetType() == typeof(T))
            {
                _currentState.Sleep();
                _currentState = _allStates[i];
                _currentState.Awake();
            }
        }
    }

    public void Update()
    {
        //Time.timeScale = 5;
        if (_currentState != null)
        {
            _currentState.Execute();
        }
    }

    public void LastUpdate()
    {
        if (_currentState != null)
        {
            _currentState.ExecuteFixed();
        }
    }
}
