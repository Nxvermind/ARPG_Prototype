using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class StaminaComponent : MonoBehaviour
{
    private PlayerParameters playerParameters;
    [SerializeField] private float regenRate;

    [SerializeField] private Image staminaBar;
    [SerializeField] private Image staminaBar_Lerp;

    private float internTimer;

    public bool OutOfStamina { get; private set; }

    // Update is called once per frame
    void Update()
    {
        staminaBar_Lerp.fillAmount = Mathf.Lerp(staminaBar_Lerp.fillAmount, staminaBar.fillAmount, Time.deltaTime * 2);

        OutOfStamina = playerParameters.currentStamina <= 0;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            ConsumeStamina(Time.deltaTime * 5);

            UpdateVisual();
            internTimer = 0;
            return;
        }

        if (playerParameters.currentStamina < playerParameters.maxStamina)
        {
            internTimer += Time.deltaTime;

            if (internTimer >= 2f)
            {
                RegenerateStamina(regenRate * Time.deltaTime);
            }
        }
        else
        {
            internTimer = 0;
        }
    }

    public void SetPlayerParameters(PlayerParameters parameters)
    {
        playerParameters = parameters;
    }

    public void ConsumeStamina(float amount)
    {
        playerParameters.currentStamina = Mathf.Max(0, playerParameters.currentStamina - amount);
        UpdateVisual();
    }

    public void RegenerateStamina(float _regenRate)
    {
        if (playerParameters.currentStamina >= playerParameters.maxStamina) return;

        playerParameters.currentStamina = Mathf.Min(playerParameters.maxStamina, playerParameters.currentStamina + _regenRate);
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        staminaBar.fillAmount = playerParameters.currentStamina / playerParameters.maxStamina;
    }

    public void ResetStaminaTimer()
    {
        internTimer = 0;
    }
}
