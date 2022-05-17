using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .BindInstance(new InputActions())
            .AsSingle();
    }
}