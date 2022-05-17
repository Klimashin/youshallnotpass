using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameplayScreen : UIScreen
{
    [SerializeField] private Button _gameOverButton;

    private GameFSM _fsm;

    [Inject]
    private void Construct(GameFSM fsm)
    {
        _fsm = fsm;

        _gameOverButton.onClick.AddListener(_fsm.OnGameOver);
    }
}
