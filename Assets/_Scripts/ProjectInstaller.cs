using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var input = new InputActions();
        input.Disable();

        Container
            .BindInstance(input)
            .AsSingle();
    }
}
