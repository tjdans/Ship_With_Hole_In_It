using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerManager player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.animator.SetBool("isRunning", false);
    }

    public override void PsUpdate()
    {
        ApplyGravity();
    }

    public override void HandleInput()
    {

        player.moveInput = player.playerInput.actions["Move"].ReadValue<Vector2>();

        if (player.moveInput.magnitude > 0.1f)
            stateMachine.ChageState(new RunState(player, stateMachine));

        if (player.playerInput.actions["Jump"].triggered)
            stateMachine.ChageState(new JumpState(player, stateMachine));
    }
}