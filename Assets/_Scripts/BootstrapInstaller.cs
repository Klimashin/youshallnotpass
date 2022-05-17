using UnityEngine;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    [SerializeField] private UIController _uiControllerPrefab;
    
    public override void InstallBindings()
    {
        Container
            .BindInterfacesAndSelfTo<UIController>()
            .FromComponentInNewPrefab(_uiControllerPrefab)
            .AsSingle();

        Container
            .BindInterfacesAndSelfTo<GameFSM>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();
    }
}
