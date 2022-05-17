using Overtime.FSM;

public enum GameStateID
{
    MAIN_MENU,
    GAMEPLAY,
}

public enum GameStateTransition
{
    MAIN_MENU_TO_GAMEPLAY,
    GAMEPLAY_TO_MAIN_MENU,
}

public abstract class GameStateBase : State<UIController, GameStateID, GameStateTransition>
{
    public override void BuildTransitions()
    { }

    public override void Enter ()
    { }

    public override void Exit ()
    { }

    public override void FixedUpdate ()
    { }

    public override void Update ()
    { }
}
