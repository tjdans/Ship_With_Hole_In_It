using UnityEngine;

public class LocomotionState : PlayerState
{
    public LocomotionState(PlayerManager player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.isGliding = false;
        player.isGlideToJump = false;
        player.animator.ResetTrigger("Land");
        player.animator.SetBool("isGliding", false);
    }

    public override void HandleInput()
    {
       player.moveInput = player.playerInput.actions["Move"].ReadValue<Vector2>();
       player.isRunning = player.playerInput.actions["Run"].ReadValue<float>() > 0.1f;

        if (player.playerInput.actions["Jump"].triggered) stateMachine.ChageState(new JumpState(player, stateMachine));
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

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(move.x, 0, move.z));
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * 15f);
        }

        // 블렌드 트리 파라미터 업데이트
        float moveAmount = player.moveInput.magnitude;

        if (player.isRunning)
        {
            // 달리기 중이면 MoveSpeed = 1
            player.animator.SetFloat("MoveSpeed", Mathf.Lerp(player.animator.GetFloat("MoveSpeed"), 1f, Time.deltaTime * 10f));
        }
        else
        {
            // 걷기 중이면 MoveSpeed = 0.5
            float target = moveAmount > 0.1f ? 0.5f : 0f;
            player.animator.SetFloat("MoveSpeed", Mathf.Lerp(player.animator.GetFloat("MoveSpeed"), target, Time.deltaTime * 10f));
        }
    }

    public override void Exit()
    {
        player.isRunning = false;
    }
}
