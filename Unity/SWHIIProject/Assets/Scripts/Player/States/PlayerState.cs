using UnityEngine;

public abstract class PlayerState
{
    protected PlayerManager player;
    protected PlayerStateMachine stateMachine;

    protected PlayerState(PlayerManager player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void HandleInput() { }
    public virtual void PsUpdate() { }

    //공통 유틸 함수들----------------------------
    public void ApplyGravity()
    {
        if (player.controller.isGrounded && player.velocity.y < 0)
        {
            player.velocity.y = -2f;
        }
        else
        {
            if (player.isGliding)
            {
                player.velocity.y += player.glideGravity * Time.deltaTime;
                player.velocity.y = Mathf.Max(player.velocity.y, -player.maxGlideFallSpeed);
            }
            else
            {
                player.velocity.y += player.normalGravity * Time.deltaTime;
            }
        }
        player.controller.Move(player.velocity * Time.deltaTime);
    }

    protected void Move(Vector3 direction)
    {
        player.controller.Move(direction * Time.deltaTime);
    }
}
