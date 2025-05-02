using Unity.VisualScripting;
using UnityEngine;

public class IdleJumpState : PlayerState
{
    private float verticalVelocity;
    private bool hasLanded;
    public IdleJumpState(PlayerManager player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }


    public override void Enter()
    {
        player.animator.SetBool("Idle", false);
        player.animator.SetTrigger("JumpFromIdle");
        player.velocity.y = Mathf.Sqrt(player.jumpForce * -2f * player.normalGravity);
    }

    public override void PsUpdate()
    {

        ApplyGravity();

        Vector3 move = new Vector3(0, player.velocity.y, 0);
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
