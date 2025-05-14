using UnityEngine;

public class RollState : PlayerState
{

    private float timer;

    public RollState(PlayerManager player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.isGliding = false;
        player.animator.SetTrigger("Roll");
        timer = 0f;
    }

    public override void HandleInput()
    {
        // 입력 무시 (점프, 이동 안됨)
    }

    public override void PsUpdate()
    {
        timer += Time.deltaTime;
        Vector3 rollDirection = player.transform.forward;
        player.controller.Move(rollDirection * player.rollSpeed * Time.deltaTime);

        if (timer >= player.rollDuration)
        {
            stateMachine.ChageState(new LocomotionState(player, stateMachine));
        }
    }
}
