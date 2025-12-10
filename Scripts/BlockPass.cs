using UnityEngine;

public class BlockPass : MonoBehaviour
{

    public GameObject colliderBlock;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            colliderBlock.SetActive(true);
        }
    }

}
