using System;

public class PlayerModel 
{
    public float MaxHP { get; }
    public float CurrentHP { get; private set; }
    
    public float MaxStamina { get; }
    public float CurrentStamina { get; private set; }
    public float RegenStaminaValue { get; }

    public float MaxUltimateSkillValue { get; }
    public float CurrentUltimateSkillValue { get; private set; }
    public float RegenUltimateSkillValue { get; }

    public float MaxPostureValue { get; }
    public float CurrentPostureValue { get; private set; }
    public float RegenPostureValue { get; }

    public event Action OnHPChangedEvent;
    public event Action OnStaminaChangedEvent;
    public event Action OnPostureChangedEvent;
    public event Action OnUltimateSkillValueChangedEvent;

    public PlayerModel(PlayerParameters parameters)
    {
        MaxHP = parameters.maxHealth;
        CurrentHP = MaxHP;

        MaxStamina = parameters.maxStamina;
        CurrentStamina = MaxStamina;
        RegenStaminaValue = parameters.regenStaminaValueSpeed;

        MaxPostureValue = parameters.maxPostureValue;
        CurrentPostureValue = MaxPostureValue;
        RegenPostureValue = parameters.regenPostureValueSpeed;

        MaxUltimateSkillValue = parameters.maxUltimateSkillValue;
        CurrentUltimateSkillValue = 0;
        RegenUltimateSkillValue = parameters.regenUltimateSkillValue;
    }

    public void IncreaseCurrentHP(float amount)
    {
        CurrentHP += amount;
        OnHPChangedEvent?.Invoke();
    }

    public void DecreaseCurrentHP(float amount)
    {
        CurrentHP -= amount;
        OnHPChangedEvent?.Invoke();
    }

    public void ResetHP()
    {
        CurrentHP = MaxHP;
        OnHPChangedEvent?.Invoke();
    }

    public void ConsumeStamina(float amount)
    {
        CurrentStamina = (float)Math.Max(0, CurrentStamina - amount);
        OnStaminaChangedEvent?.Invoke();
    }

    public void RestoreStamina(float regenRate)
    {
        if (CurrentStamina >= MaxStamina) return;

        CurrentStamina = (float)Math.Min(MaxStamina, CurrentStamina + regenRate);

        OnStaminaChangedEvent?.Invoke();
    }

    public void ResetStamina()
    {
        CurrentStamina = MaxStamina;
        OnStaminaChangedEvent?.Invoke();
    }

    public void DecreasePostureValue(float amount)
    {
        CurrentPostureValue -= amount;
        OnPostureChangedEvent?.Invoke();
    }

    public void ResetPostureValue()
    {
        CurrentPostureValue = MaxPostureValue;
        OnPostureChangedEvent?.Invoke();
    }

    public void RestorePosture(float regenRate)
    {
        if (CurrentPostureValue >= MaxPostureValue) return;

        CurrentPostureValue = (float)Math.Min(MaxPostureValue, CurrentPostureValue + regenRate);

        OnPostureChangedEvent?.Invoke();
    }

    public void IncreaseUltimateSkillValue(float regenUltimateSkillValue)
    {
        CurrentUltimateSkillValue += regenUltimateSkillValue;
        OnUltimateSkillValueChangedEvent?.Invoke();
    }

    public void ResetUltimateSkillValue()
    {
        CurrentUltimateSkillValue = 0;
        OnUltimateSkillValueChangedEvent?.Invoke();
    }
}
