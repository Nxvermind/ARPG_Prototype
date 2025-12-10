using Unity.Cinemachine;
using UnityEngine;

public class FarClipPlaneModifier : MonoBehaviour
{
    public CinemachineCamera[] cameras;

    public bool isFarClipIncreased;

    private void Start()
    {
        isFarClipIncreased = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isFarClipIncreased)
            {
                ModifyFarClipPlane(100);
                isFarClipIncreased = !isFarClipIncreased;
            }
            else
            {
                isFarClipIncreased = !isFarClipIncreased;
                ModifyFarClipPlane(180);
            }
        }
    }

    private void ModifyFarClipPlane(int value)
    {
        foreach (var cam in cameras)
        {
            cam.Lens.FarClipPlane = value;
        }
    }
}
