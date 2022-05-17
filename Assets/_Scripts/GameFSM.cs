using UnityEngine;
using Zenject;
using MonsterLove.StateMachine;

public class GameFSM : MonoBehaviour, IInitializable
{
    private StateMachine<States, Driver> _fsm;
    private UIController _uiController;
    private InputActions _input;

    public void Initialize()
    {
        _fsm = new StateMachine<States, Driver>(this);
        _fsm.ChangeState(States.MainMenu);
    }

    [Inject]
    private void Construct(UIController uiController, InputActions input)
    {
        _uiController = uiController;
        _input = input;
    }

    public void OnStartGameButtonPressed()
    {
        _fsm.Driver.MainMenuToGameplay.Invoke();
    }

    public void OnMainMenuButtonPressed()
    {
        _fsm.Driver.GameOverToMainMenu.Invoke();
    }

    public void OnRestartButtonPressed()
    {
        _fsm.Driver.GameOverToGameplay.Invoke();
    }

    public void OnGameOver()
    {
        _fsm.Driver.GameplayToGameOver.Invoke();
    }

    private void Update()
    {
        _fsm?.Driver.Update.Invoke();
    }

    #region FSM states

    private enum States
    {
        MainMenu,
        Gameplay,
        GameOver
    }

    private class Driver
    {
        public StateEvent Update;
        public StateEvent MainMenuToGameplay;
        public StateEvent GameplayToGameOver;
        public StateEvent GameOverToMainMenu;
        public StateEvent GameOverToGameplay;
    }

    #region MainMenuState
    private void MainMenu_Enter()
    {
        _uiController.ShowUIElement<MainMenuScreen>();
    }

    private void MainMenu_MainMenuToGameplay()
    {
        _fsm.ChangeState(States.Gameplay);
    }

    private void MainMenu_Exit()
    {
        _uiController.HideUIElement<MainMenuScreen>();
    }
    #endregion

    #region GameplayState
    private void Gameplay_Enter()
    {
        _uiController.ShowUIElement<GameplayScreen>();
        _input.Gameplay.Enable();
    }

    private void Gameplay_GameplayToGameOver()
    {
        _fsm.ChangeState(States.GameOver);
    }

    private void Gameplay_Exit()
    {
        _input.Gameplay.Disable();
        _uiController.HideUIElement<GameplayScreen>();
    }
    #endregion

    #region GameOverState
    private void GameOver_Enter()
    {
        _uiController.ShowUIElement<GameOverScreen>();
    }

    private void GameOver_GameOverToMainMenu()
    {
        _fsm.ChangeState(States.MainMenu);
    }

    private void GameOver_GameOverToGameplay()
    {
        _fsm.ChangeState(States.Gameplay);
    }

    private void GameOver_Exit()
    {
        _uiController.HideUIElement<GameOverScreen>();
    }
    #endregion

    #endregion
}
