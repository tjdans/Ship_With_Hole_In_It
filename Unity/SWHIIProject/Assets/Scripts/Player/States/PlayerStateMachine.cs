using UnityEngine;

//상태 전환만을 담당하는 클래스
public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }

    public void Initialize(PlayerState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }
   
    public void ChageState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void SmUpdate()
    {
        currentState?.HandleInput();
        currentState?.PsUpdate();
    }
}
