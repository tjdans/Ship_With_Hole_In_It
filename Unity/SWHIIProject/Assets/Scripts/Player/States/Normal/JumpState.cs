using UnityEngine;

public class JumpState : PlayerState
{

    Vector3 moveDirection;
    bool allowAirControl = false;

    public JumpState(PlayerManager player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        if (!player.isGlideToJump)
        // 점프 초기 속도 적용
        player.velocity.y = Mathf.Sqrt(player.jumpForce * -2f * player.normalGravity);

        // 달리기 중 점프이면 이동 허용
        allowAirControl = player.animator.GetFloat("MoveSpeed") > 0.5f ? true : false;
        // 애니메이션 전환
        if (player.isBattle) player.animator.SetTrigger(allowAirControl ? "JumpFromBattleRun" : "JumpFromBattleIdle");

        else if (!player.isBattle) player.animator.SetTrigger(allowAirControl ? "JumpFromRun" : "JumpFromIdle");
    }

    public override void HandleInput()
    {
        player.moveInput = player.playerInput.actions["Move"].ReadValue<Vector2>();

        if (player.playerInput.actions["Jump"].triggered)
        {
            if(player.transform.position.y > 2.0f)
            {
                stateMachine.ChageState(new GlideState(player, stateMachine));
            }
        }
    }

    public override void PsUpdate()
    {
        // 공중 이동 허용 시 처리
        if (allowAirControl)
        {
            Vector3 input = new Vector3(player.moveInput.x, 0f, player.moveInput.y);
            moveDirection = player.cameraTransform.forward * input.z + player.cameraTransform.right * input.x;
            moveDirection.y = 0f;
            moveDirection.Normalize();

            Vector3 horizontalMovement = moveDirection * player.moveSpeed;
            player.controller.Move(horizontalMovement * Time.deltaTime);

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * 15f);
            }
        }

        // 중력 적용
        ApplyGravity();

        // 착지 체크
        if (player.controller.isGrounded && player.velocity.y <= 0f)
        {
            player.isGliding = false;
            player.animator.SetTrigger("Land");
        }
    }
}