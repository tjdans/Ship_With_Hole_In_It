using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeIdleState : PlayerState
{
    public MeleeIdleState(PlayerManager player, PlayerStateMachine machine) : base(player, machine) { }

    public override void Enter()
    {
        player.isBattle = true;
        player.animator.SetBool("WeaponIdle", player.isBattle);
    }

    public override void Exit()
    {

    }

    public override void PsUpdate()
    {
       ApplyGravity();
    }

    public override void HandleInput()
    {


        player.moveInput = player.playerInput.actions["Move"].ReadValue<Vector2>();

        if (player.moveInput.magnitude > 0.1f)
        {

        }
            

        if (player.playerInput.actions["Jump"].triggered)
            stateMachine.ChageState(new JumpState(player, stateMachine));
    }
}
