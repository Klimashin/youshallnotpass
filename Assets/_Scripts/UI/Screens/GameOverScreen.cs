using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameOverScreen : UIScreen
{
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _restartButton;

    private GameFSM _fsm;

    [Inject]
    private void Construct(GameFSM fsm)
    {
        _fsm = fsm;

        _mainMenuButton.onClick.AddListener(_fsm.OnMainMenuButtonPressed);
        _restartButton.onClick.AddListener(_fsm.OnRestartButtonPressed);
    }
}
