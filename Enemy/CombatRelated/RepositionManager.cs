using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RepositionManager : MonoBehaviour
{
    public float totalAngle;
    public float frontSideAngle;
    public float rearSideAngle;
    public float leftSideAngle;
    public float rightSideAngle;

    [Header("Slots capacity")]
    public int maxEnemiesInFrontSide;
    public int maxEnemiesInLeftSide;
    public int maxEnemiesInRightSide;

    [Header("Radius")]
    public float minRadius;
    public float middleRadius;
    public float rearRadius;
    public float maxRadius;

    [Header("Slot exclusion")]
    public float slotExclusionRadius;

    public List<EnemyReposition> registered = new();
    public List<EnemyReposition> FrontSide= new();
    public List<EnemyReposition> LeftSide= new();
    public List<EnemyReposition> RightSide = new();

    public void Register(EnemyReposition enemy)
    {
        if (!registered.Contains(enemy))
        {
            registered.Add(enemy);
        }
    }
    public void Unregister(EnemyReposition enemy)
    {
        registered.Remove(enemy);
    }
    public void AssignToFrontSide(EnemyReposition enemy)
    {
        FrontSide.Add(enemy);
    }
    public void AssignToLeftSide(EnemyReposition enemy)
    {
        LeftSide.Add(enemy);
    }
    public void AssignToRightSide(EnemyReposition enemy)
    {
        RightSide.Add(enemy);
    }
    public void RemoveFromSideSpace(EnemyReposition enemy)
    {
        if (FrontSide.Contains(enemy))
        {
            FrontSide.Remove(enemy);
        }
        else if (RightSide.Contains(enemy))
        {
            RightSide.Remove(enemy);
        }
        else if (LeftSide.Contains(enemy))
        {
            LeftSide.Remove(enemy);
        }
    }
    public bool IsSlotAvailable(Vector3 point, EnemyReposition requester)
    {
        float sqrExclusion = slotExclusionRadius * slotExclusionRadius;

        for (int i = 0; i < registered.Count; i++)
        {
            EnemyReposition other = registered[i];

            if (other == requester)
                continue;

            if (!other.HasSlot)
                continue;

            Vector3 otherSlot = other.CurrentSlot;

            if ((point - otherSlot).sqrMagnitude < sqrExclusion) return false;
        }

        return true;
    }
    public bool HasInnerRingSpace()
    {
        return FrontSide.Count < maxEnemiesInFrontSide ||
               LeftSide.Count < maxEnemiesInLeftSide   ||
               RightSide.Count < maxEnemiesInRightSide;
    }
}
