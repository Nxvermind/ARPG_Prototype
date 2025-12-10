using System;

public static class EventBus 
{
    public static event Action OnEnemyHitEvent;

    public static event Action<Enemy> OnEnemyGotHitEvent;

    public static event Action<Enemy> OnEnemyDeathEvent;

    public static event Action OnExecutionStarted;

    public static void EnemyHitEvent()
    {
        OnEnemyHitEvent?.Invoke();
    }

    public static void EnemyGotHitEvent(Enemy enemy)
    {
        OnEnemyGotHitEvent?.Invoke(enemy);
    }

    public static void StartExecution()
    {
        OnExecutionStarted?.Invoke();
    }

    public static void EnemyDeathEvent(Enemy enemy)
    {
        OnEnemyDeathEvent?.Invoke(enemy);
    }


}
