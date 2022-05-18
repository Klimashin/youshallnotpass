using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private GameSettings _gameSettings;
    
    public override void InstallBindings()
    {
        var input = new InputActions();
        input.Disable();

        Container
            .BindInstance(input)
            .AsSingle();

        Container
            .BindInstance(new PersistentStorage())
            .AsSingle();

        Container
            .BindInstance(_gameSettings)
            .AsSingle();
    }
}
