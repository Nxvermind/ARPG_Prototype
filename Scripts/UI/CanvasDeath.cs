using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasDeath : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject lastCheckPointButton;
    [SerializeField] private CanvasGroup canvasGroup;

    public ActivateElite activateElite;

    public bool canvasDeathShowed;
    private void Start()
    {
        canvasDeathShowed = false;
    }

    private void OnEnable()
    {
        PlayerEvents.OnPlayerDeath += ActivatePanel;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerDeath -= ActivatePanel;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void ActivatePanel()
    {
        panel.SetActive(true);
        canvasDeathShowed = true;
        StartCoroutine(Cor());

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (activateElite.lastCheckPointAvailable)
        {
            lastCheckPointButton.SetActive(true);
        }
    }

    public void DeactivatePanel()
    {
        panel.SetActive(false);
        canvasDeathShowed = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator Cor()
    {
        float timeElapsed = 0;
        float duration = 1f;

        while(timeElapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0,1, timeElapsed/ duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

}
