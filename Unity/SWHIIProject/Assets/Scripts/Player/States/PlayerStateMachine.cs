using UnityEngine;

//���� ��ȯ���� ����ϴ� Ŭ����
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
