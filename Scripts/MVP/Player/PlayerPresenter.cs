using UnityEngine;

public class PlayerPresenter
{
    private readonly PlayerView view;
    private readonly PlayerModel model;

    public PlayerPresenter(PlayerModel _playerModel, PlayerView _playerView)
    {
        model = _playerModel;
        view = _playerView;

        model.OnHPChangedEvent += view.UpdateHealthView;
        model.OnPostureChangedEvent += view.UpdatePostureView;
        model.OnStaminaChangedEvent += view.UpdateStaminaView;
        model.OnStaminaChangedVariantEvent += view.UpdateStaminaViewVariant;
        model.OnUltimateSkillValueChangedEvent += view.UpdateUltimateSkillBarView;

        view.UpdateHealthView(model.CurrentHP, model.MaxHP);
        view.UpdatePostureView(model.CurrentPostureValue, model.MaxPostureValue);
        view.UpdateStaminaView(model.CurrentStamina, model.MaxStamina);
        view.UpdateUltimateSkillBarView(model.CurrentUltimateSkillValue, model.MaxUltimateSkillValue);
    }
}
