using UnityEngine;

public class GameplayState : GameStateBase
{
    public override void BuildTransitions()
    {
        AddTransition(GameStateTransition.GAMEPLAY_TO_MAIN_MENU, GameStateID.MAIN_MENU);
    }

    public override void Enter()
    {
        Debug.Log("GAMEPLAY_STATE");
    }
}
