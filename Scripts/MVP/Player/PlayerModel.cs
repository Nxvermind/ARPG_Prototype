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

    public event Action<float, float> OnHPChangedEvent;
    public event Action<float, float> OnStaminaChangedEvent;
    public event Action<float, float> OnStaminaChangedVariantEvent;
    public event Action<float, float> OnPostureChangedEvent;
    public event Action<float, float> OnUltimateSkillValueChangedEvent;

    public PlayerModel(PlayerParameters parameters)
    {
        MaxHP = parameters.maxHP;
        CurrentHP = MaxHP;

        MaxStamina = parameters.maxStamina;
        CurrentStamina = MaxStamina;
        RegenStaminaValue = parameters.regenStaminaValue;

        MaxPostureValue = parameters.maxPostureValue;
        CurrentPostureValue = MaxPostureValue;
        RegenPostureValue = parameters.regenPostureValue;

        MaxUltimateSkillValue = parameters.maxUltimateSkillValue;
        CurrentUltimateSkillValue = parameters.currentUltimateSkillValue;
        RegenUltimateSkillValue = parameters.regenUltimateSkillValue;
    }

    public void UpdateHP(float newHP)
    {
        CurrentHP = newHP;
        OnHPChangedEvent?.Invoke(CurrentHP, MaxHP);
    }

    public void ConsumeStamina(float amount)
    {
        CurrentStamina = (float)Math.Max(0, CurrentStamina - amount);
        OnStaminaChangedEvent?.Invoke(CurrentStamina, MaxStamina);
    }

    public void RestoreStamina(float regenRate)
    {
        if (CurrentStamina >= MaxStamina) return;

        CurrentStamina = (float)Math.Min(MaxStamina, CurrentStamina + regenRate);

        OnStaminaChangedVariantEvent?.Invoke(CurrentStamina, MaxStamina);
    }

    public void UpdatePostureValue(float amount)
    {
        CurrentPostureValue -= amount;
        OnPostureChangedEvent?.Invoke(CurrentPostureValue, MaxPostureValue);
    }

    public void ResetPostureValue()
    {
        CurrentPostureValue = MaxPostureValue;
        OnPostureChangedEvent?.Invoke(CurrentPostureValue, MaxPostureValue);
    }

    public void RegeneratePostureValue(float regenRate)
    {
        if (CurrentPostureValue >= MaxPostureValue) return;

        CurrentPostureValue = (float)Math.Min(MaxPostureValue, CurrentPostureValue + regenRate);

        OnPostureChangedEvent?.Invoke(CurrentPostureValue, MaxPostureValue);
    }

    public void UpdateUltimateSkillValue(float regenUltimateSkillValue)
    {
        CurrentUltimateSkillValue += regenUltimateSkillValue;
        OnUltimateSkillValueChangedEvent?.Invoke(CurrentUltimateSkillValue, MaxUltimateSkillValue);
    }

    public void ResetUltimateSkillValue()
    {
        CurrentUltimateSkillValue = 0;
        OnUltimateSkillValueChangedEvent?.Invoke(CurrentUltimateSkillValue, MaxUltimateSkillValue);
    }
}
