using Overtime.FSM;
using UnityEngine;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    [SerializeField] private UIController _uiControllerPrefab;
    [SerializeField] private StateMachineSettings _stateMachineSettings;
    
    public override void InstallBindings()
    {
        UIController uiController = Instantiate(_uiControllerPrefab);

        Container
            .BindInstance(uiController)
            .AsSingle();
        
        var stateMachine = new StateMachine<UIController, GameStateID, GameStateTransition>(
            uiController,
            _stateMachineSettings.States,
            _stateMachineSettings.InitialState,
            _stateMachineSettings.Debug);

        Container
            .BindInstance(stateMachine)
            .AsSingle();
    }
}

[System.Serializable]
public class StateMachineSettings
{
    public GameStateID InitialState;
    public ScriptableObject[] States;
    public bool Debug;
}
