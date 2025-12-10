using UnityEngine;

/// <summary>
/// Base abstract state used by the generic FSM.  
/// Represents a single behavioral mode for an entity and provides lifecycle hooks (Enter, Update, Exit) that derived states override. 
/// Supports both player and enemy workflows by allowing construction with or without an EnemyStateFactory for modular state access.
/// </summary>
public abstract class State<T> where T : MonoBehaviour
{
    protected T entity;
    protected StateMachine<T> stateMachine;
    protected EnemyStateFactory factory;

    //for the player
    public State(T entity, StateMachine<T> stateMachine)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
    }

    //for enemies
    public State(T entity, EnemyStateFactory enemyStateFactory, StateMachine<T> stateMachine)
    {
        this.entity = entity;
        this.factory = enemyStateFactory;
        this.stateMachine = stateMachine;   
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
