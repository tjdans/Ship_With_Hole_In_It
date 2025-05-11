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

        // ���߿��� �ٽ� �����̽��� ������ ���� ���� (��� �۶��̵� ���� + �߷� ���븸)
        if (player.playerInput.actions["Jump"].triggered)
        {
            player.animator.SetBool("isGliding", false);
            stateMachine.ChageState(new JumpState(player, stateMachine));
        }
    }

    public override void PsUpdate()
    {

        // WŰ�� �̵� ���
        if (player.moveInput.y > 0.1f && Mathf.Abs(player.moveInput.x) < 0.1f)
        {
            moveDirection = player.cameraTransform.forward;
            moveDirection.y = 0f;
            moveDirection.Normalize();

            player.controller.Move(moveDirection * player.glideSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // �߷� ����
        ApplyGravity();

        // ���� �� Roll ���·� ��ȯ
        if (player.controller.isGrounded && player.velocity.y <= 0f)
        {
            player.animator.SetTrigger("Roll");
            stateMachine.ChageState(new RollState(player, stateMachine));
        }
    }
}