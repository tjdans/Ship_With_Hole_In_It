using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeIdleState : WeaponState
{
    public MeleeIdleState(PlayerManager player, PlayerStateMachine machine) : base(player, machine) { }

    public override void Enter()
    {
        player.animator.SetBool("WeaponIdle", true);
    }

    public override void Exit()
    {
        player.animator.SetBool("WeaponIdle", false);
    }

    public override void PsUpdate()
    {
       ApplyGravity();



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
