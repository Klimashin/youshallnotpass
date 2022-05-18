using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private UIController _uiControllerPrefab;
    [SerializeField] private GameplayController _gameplayController;
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject WeaponPrefab;
    
    public override void InstallBindings()
    {
        Container
            .BindInterfacesAndSelfTo<UIController>()
            .FromComponentInNewPrefab(_uiControllerPrefab)
            .AsSingle();

        Container
            .BindInstance(_gameplayController)
            .AsSingle();

        Container
            .BindInterfacesAndSelfTo<GameFSM>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();
    }
}
