using UnityEngine;

public class RunJumpState : PlayerState
{
    private float verticalVelocity;

    public RunJumpState(PlayerManager player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.animator.SetTrigger("JumpFromRun");
        player.velocity.y = Mathf.Sqrt(player.jumpForce * -2f * player.normalGravity);
    }

    public override void PsUpdate()
    {
        Vector3 move = new Vector3(player.moveInput.x, 0, player.moveInput.y);
        move = Quaternion.Euler(0, player.cameraTransform.eulerAngles.y, 0) * move;
        move.Normalize();
        move *= player.moveSpeed;

        ApplyGravity();
        move.y = player.velocity.y;

        Move(move);

        if (player.controller.isGrounded && player.velocity.y <= 0f)
        {
            player.animator.SetTrigger("Land");

            if (player.moveInput.magnitude > 0.1f)
                stateMachine.ChageState(new RunState(player, stateMachine));
            else
                stateMachine.ChageState(new IdleState(player, stateMachine));
        }
        else if (player.playerInput.actions["Jump"].triggered)
        {
            stateMachine.ChageState(new GlideState(player, stateMachine));
        }
    }
}
