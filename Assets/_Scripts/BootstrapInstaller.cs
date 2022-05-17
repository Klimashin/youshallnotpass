using UnityEngine;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    [SerializeField] private UIController _uiControllerPrefab;
    
    public override void InstallBindings()
    {
        Container
            .Bind<UIController>()
            .FromComponentInNewPrefab(_uiControllerPrefab)
            .AsSingle()
            .NonLazy();

        Container
            .Bind<GameFSM>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();
    }
}
