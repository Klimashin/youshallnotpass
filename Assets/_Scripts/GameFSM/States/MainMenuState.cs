using Zenject;

public class MainMenuState : GameStateBase
{
    public override void BuildTransitions()
    {
        AddTransition(GameStateTransition.MAIN_MENU_TO_GAMEPLAY, GameStateID.GAMEPLAY);
    }

    public override void Enter()
    {
        Parent.ShowUIElement<MainMenuScreen>();
    }
}
