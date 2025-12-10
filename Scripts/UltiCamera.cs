using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class UltiCamera : MonoBehaviour
{
    [SerializeField] private SwitchCameras switchCameras;
    [SerializeField] private UltimateSkill_UI ultimateSkill_UI;

    [Space]
    public CinemachineCamera[] firstAndLastUltimateCameras;

    public bool skillCasted;

    private void OnEnable()
    {
        PlayerEvents.OnUltimateSkillCalled += StartCast;
    }

    private void OnDisable()
    {
        PlayerEvents.OnUltimateSkillCalled -= StartCast;
    }


    private void StartCast()
    {
        StartCoroutine(CastUltiSkill());
    }

    IEnumerator CastUltiSkill()
    {
        skillCasted = true;
        switchCameras.CurrentActiveCinemachineCamera.Priority = 10;

        firstAndLastUltimateCameras[0].Priority = 80;

        var cam = firstAndLastUltimateCameras[0];

        var pos = cam.State.GetFinalPosition();
        var rot = cam.State.GetFinalOrientation();

        switchCameras.CurrentActiveCinemachineCamera.ForceCameraPosition(pos, rot);

        yield return new WaitForSecondsRealtime(0.05f);

        firstAndLastUltimateCameras[1].Priority = 90;

        yield return new WaitForSecondsRealtime(1.75f);
            

        firstAndLastUltimateCameras[0].Priority = 1;
        firstAndLastUltimateCameras[1].Priority= 1;

        switchCameras.CurrentActiveCinemachineCamera.Priority = 40;

        skillCasted = false;
    }

}
