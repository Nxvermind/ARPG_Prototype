using System.Collections;
using UnityEngine;

public class ShowPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            panel.SetActive(true);

            gameObject.SetActive(false);
        }
    }
}
