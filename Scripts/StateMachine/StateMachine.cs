using UnityEngine;

/// <summary>
/// Generic Finite State Machine for Unity MonoBehaviour entities.
/// Manages the current state, preserves the previous state, and ensures proper Enter/Exit calls during transitions. 
/// </summary>

public class StateMachine<T> where T : MonoBehaviour 
{
    public State<T> CurrentState { get; private set; }

    public State<T> PreviousState { get; private set; }
    
    public void Initialize(State<T> state)
    {
        CurrentState = state;
        CurrentState.Enter();
    }

    public void ChangeState(State<T> newState)
    {
        CurrentState.Exit();

        PreviousState = CurrentState;

        CurrentState = newState;
        CurrentState.Enter();
    }
}
