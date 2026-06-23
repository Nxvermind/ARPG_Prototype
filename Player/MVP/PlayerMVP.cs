using UnityEngine;

public class PlayerMVP
{
    private PlayerParameters parameters;

    private PlayerView playerView;
    public PlayerModel PlayerModel { get; private set; }
    public PlayerPresenter PlayerPresenter { get; private set; }

    public PlayerMVP(PlayerParameters parameters, PlayerView playerView)
    {
        this.parameters = parameters;
        this.playerView = playerView;
    }

    public void Initialize()
    {
        PlayerModel = new PlayerModel(parameters);
        PlayerPresenter = new PlayerPresenter(PlayerModel, playerView);
    }
}
