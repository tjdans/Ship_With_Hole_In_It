using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeAttack2State : WeaponState
{
    public MeleeAttack2State(PlayerManager player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }


    public override void Enter()
    {
        player.comboStep++;
        player.animator.SetTrigger("MeleeAttack" + player.comboStep);

        if (player.comboStep >= 2)
            player.comboStep = 0;
    }

    public override void PsUpdate()
    {
        ApplyGravity();
        if (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
        {
            stateMachine.ChageState(new MeleeIdleState(player, stateMachine));
        }
    }

    public override void HandleInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            stateMachine.ChageState(new MeleeAttack1State(player, stateMachine));
        }

        player.moveInput = player.playerInput.actions["Move"].ReadValue<Vector2>();

        if (player.moveInput.magnitude > 0.1f)
            stateMachine.ChageState(new RunState(player, stateMachine));

        if (player.playerInput.actions["Jump"].triggered)
            stateMachine.ChageState(new JumpState(player, stateMachine));
    }

}
