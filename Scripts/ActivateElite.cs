using UnityEngine;

public class ActivateElite : MonoBehaviour
{
    public GameObject eliteEnemy;

    public bool lastCheckPointAvailable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            eliteEnemy.SetActive(true);
            lastCheckPointAvailable = true;
        }
    }

}
