using UnityEngine;

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
