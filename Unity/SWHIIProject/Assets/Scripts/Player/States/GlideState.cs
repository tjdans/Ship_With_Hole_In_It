using UnityEngine;

public class GlideState : PlayerState
{
    public GlideState(PlayerManager player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    Vector3 moveDirection;

    public override void Enter()
    {
        player.isGliding = true;
        player.animator.SetBool("isGliding", true);
    }

    public override void Exit()
    {
        player.animator.SetBool("isGliding", false);
    }

    public override void HandleInput()
    {
        player.moveInput = player.playerInput.actions["Move"].ReadValue<Vector2>();

        // 공중에서 다시 스페이스바 누르면 강제 낙하 (즉시 글라이딩 종료 + 중력 적용만)
        if (player.playerInput.actions["Jump"].triggered)
        {
            player.animator.SetBool("isGliding", false);
            stateMachine.ChageState(new JumpState(player, stateMachine));
        }
    }

    public override void PsUpdate()
    {

        // W키만 이동 허용
        if (player.moveInput.y > 0.1f && Mathf.Abs(player.moveInput.x) < 0.1f)
        {
            moveDirection = player.cameraTransform.forward;
            moveDirection.y = 0f;
            moveDirection.Normalize();

            player.controller.Move(moveDirection * player.glideSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // 중력 적용
        ApplyGravity();

        // 착지 시 Roll 상태로 전환
        if (player.controller.isGrounded && player.velocity.y <= 0f)
        {
            player.animator.SetTrigger("Roll");
            stateMachine.ChageState(new RollState(player, stateMachine));
        }
    }
}