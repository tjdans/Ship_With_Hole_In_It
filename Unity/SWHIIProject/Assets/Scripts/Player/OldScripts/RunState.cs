using UnityEngine;

public class RunState : PlayerState
{
    public RunState(PlayerManager player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {

    }

    public override void HandleInput()
    {
        player.moveInput = player.playerInput.actions["Move"].ReadValue<Vector2>();
        player.isRunning = player.playerInput.actions["Run"].ReadValue<float>() > 0.1;

        if (player.moveInput.magnitude <= 0.1f)
        {
            player.animator.SetBool("isRunning", false);
            player.animator.SetBool("isWalking", false);
            stateMachine.ChageState(new IdleState(player, stateMachine));
        }
        else
        {
            player.animator.SetBool("isRunning", player.isRunning);
            player.animator.SetBool("isWalking", !player.isRunning);
        }

        if (player.playerInput.actions["Jump"].triggered)
        {

            stateMachine.ChageState(new JumpState(player, stateMachine));
        }
    }

    public override void PsUpdate()
    {

        Vector3 move = new Vector3(player.moveInput.x, 0, player.moveInput.y);
        move = Quaternion.Euler(0, player.cameraTransform.eulerAngles.y, 0) * move;
        move.Normalize();

        float speed = player.isRunning ? player.runSpeed : player.moveSpeed;
        move *= speed;

        ApplyGravity();
        move.y = player.velocity.y;

        Move(move);

        // È¸Àü
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(move.x, 0, move.z));
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * 15f);
        }

    }
}
