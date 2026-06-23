using UnityEngine;

public class PlayerPresenter
{
    private readonly PlayerView view;
    private readonly PlayerModel model;

    public PlayerPresenter(PlayerModel _playerModel, PlayerView _playerView)
    {
        model = _playerModel;
        view = _playerView;

        model.OnHPChangedEvent += UpdateHealth;
        model.OnPostureChangedEvent += UpdatePosture;
        model.OnStaminaChangedEvent += UpdateStamina;
        model.OnUltimateSkillValueChangedEvent += UpdateUltimateSkillValue;
    }

    private void UpdateHealth() => view.UpdateHealthView(model.CurrentHP, model.MaxHP);
    private void UpdatePosture() => view.UpdatePostureView(model.CurrentPostureValue, model.MaxPostureValue);
    private void UpdateStamina() => view.UpdateStaminaView(model.CurrentStamina, model.MaxStamina);
    private void UpdateUltimateSkillValue() => view.UpdateUltimateSkillBarView(model.CurrentUltimateSkillValue, model.MaxUltimateSkillValue);
}
