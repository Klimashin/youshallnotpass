using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameplayScreen : UIScreen
{
    [SerializeField] private Button _gameOverButton;

    private GameFSM _fsm;
    private GameplayController _gameplayCtrl;

    [Inject]
    private void Construct(GameFSM fsm, GameplayController gameplayCtrl)
    {
        _fsm = fsm;
        _gameplayCtrl = gameplayCtrl;

        _gameOverButton.onClick.AddListener(_fsm.OnGameOver);
    }

    protected override void OnPreShow()
    {
        base.OnPreShow();

        _gameplayCtrl.StartGameplaySequence();
    }
}
