using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class IntroScript : MonoBehaviour
{
    [SerializeField] private CanvasDeath canvasDeath;
    [SerializeField] private GameObject panelIntro;
    public GameObject panelToMove;
    public GameObject panelToUlti;

    public GameObject panelToOptions;

    public GameObject checkBool;

    public static event Action OnIntroStarted;

    public static event Action OnIntroFinished;

    public bool introStarted;
    public bool optionsShowed;

    private void Update()
    {
        if (canvasDeath.canvasDeathShowed) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            panelToOptions.SetActive(true);

            Time.timeScale = 0;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            optionsShowed = true;
        }
    }

    public void StartButonExecuted()
    {
        panelIntro.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        OnIntroStarted?.Invoke();

        introStarted = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void EnterOptions()
    {
        panelToOptions.SetActive(true);
    }

    public void ExitOptions()
    {
        panelToOptions.SetActive(false);

        optionsShowed = false;

        Time.timeScale = 1;

        if (introStarted)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }

    public void EnableVSing()
    {
        checkBool.SetActive(true);
        QualitySettings.vSyncCount = 1;
    }

    public static void IntroFinishedEvent()
    {
        OnIntroFinished?.Invoke();
    }
}
