using UnityEngine;

public class EnemyParryStatus : MonoBehaviour
{
    public bool CanBeParried { get; private set; }

    public bool isParried;

    public void EnableParryWindow()
    {
        CanBeParried = true;
    }

    public void DisableParryWindow()
    {
        CanBeParried = false;
    }

}
