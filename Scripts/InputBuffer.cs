using System.Collections.Generic;
using UnityEngine;


public enum AttackType
{
    None,
    Light,
    Heavy
}

public class InputBuffer
{
    private float bufferedTime;
    private int maxBufferSize = 1;
    public Queue<AttackType> queue = new Queue<AttackType>();
    public AttackType attackType;

    public void RegisterInput(float time,AttackType type)
    {
        if(queue.Count >= maxBufferSize)
        {
            return;
        }

        queue.Enqueue(type);
        bufferedTime = time;
        attackType = type;
    }

    public void CountDown(float deltaTime)
    {
        if(queue.Count <= 0)
        {
            return;
        }

        bufferedTime -= deltaTime;

        if (bufferedTime <= 0)
        {
            ClearInput();
        }
    }

    public void ClearInput()
    {
        queue.Dequeue();
        bufferedTime = 0;
        attackType = AttackType.None;
    }
}
