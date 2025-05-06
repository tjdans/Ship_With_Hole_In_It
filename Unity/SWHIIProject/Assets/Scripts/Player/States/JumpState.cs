using UnityEngine;

public class JumpState : PlayerState
{

    Vector3 moveDirection;
    bool allowAirControl = false;

    public JumpState(PlayerManager player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    Vector3 move = new Vector3(0, 0, 0);

    public override void Enter()
    {
        if(!player.isGliding)
        // ���� �ʱ� �ӵ� ����
        player.velocity.y = Mathf.Sqrt(player.jumpForce * -2f * player.normalGravity);

        // �޸��� �� �����̸� �̵� ���
        allowAirControl = player.moveInput.magnitude > 0.1f;

        // �ִϸ��̼� ��ȯ
        player.animator.SetTrigger(allowAirControl ? "JumpFromRun" : "JumpFromIdle");
        player.animator.SetBool("Idle", false);
    }

    public override void HandleInput()
    {
        player.moveInput = player.playerInput.actions["Move"].ReadValue<Vector2>();

        if (player.playerInput.actions["Jump"].triggered)
        {
            stateMachine.ChageState(new GlideState(player, stateMachine));
        }
    }

    public override void PsUpdate()
    {
        // ���� �̵� ��� �� ó��
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

        // �߷� ����
        ApplyGravity();

        // ���� üũ
        if (player.controller.isGrounded && player.velocity.y <= 0f)
        {
            player.isGliding = false;
            player.animator.SetTrigger("Land");
            stateMachine.ChageState(player.moveInput.magnitude > 0.1f
                ? new RunState(player, stateMachine)
                : new IdleState(player, stateMachine));
        }
    }
}