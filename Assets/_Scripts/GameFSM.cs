using UnityEngine;
using MonsterLove.StateMachine;
using Zenject;

public class GameFSM : MonoBehaviour
{
    public enum States
    {
        MainMenu,
        Gameplay
    }

    private StateMachine<States, StateDriverUnity> _fsm;
    private UIController _uiController;

    [Inject]
    private void Construct(UIController uiController)
    {
        _uiController = uiController;
    }

    private void Awake()
    {
        _fsm = new StateMachine<States, StateDriverUnity>(this);
        _fsm.ChangeState(States.MainMenu);
    }

    void MainMenu_Enter()
    {
        _uiController.ShowUIElement<MainMenuScreen>();
    }
}
