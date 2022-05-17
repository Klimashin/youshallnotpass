using Overtime.FSM;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuScreen : UIScreen
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _quitGameButton;

    private StateMachine<UIController, GameStateID, GameStateTransition> _gameFsm;
    
    [Inject]
    private void Construct(StateMachine<UIController, GameStateID, GameStateTransition> fsm)
    {
        _gameFsm = fsm;
    }

    protected override void OnPostShow()
    {
        _startGameButton.onClick.AddListener(OnStartGameButtonClick);
        _quitGameButton.onClick.AddListener(OnQuitGameButtonClick);
    }
    
    protected override void OnPreHide()
    {
        _startGameButton.onClick.RemoveAllListeners();
        _quitGameButton.onClick.RemoveAllListeners();
    }

    private void OnQuitGameButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void OnStartGameButtonClick()
    {
        UIController.ToGameplayState();
    }
}
