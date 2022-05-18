using Zenject;

public class GameplayScreen : UIScreen
{
    private GameplayController _gameplayCtrl;

    [Inject]
    private void Construct(GameplayController gameplayCtrl)
    {
        _gameplayCtrl = gameplayCtrl;
    }

    protected override void OnPreShow()
    {
        base.OnPreShow();

        _gameplayCtrl.StartGameplaySequence();
    }
}
