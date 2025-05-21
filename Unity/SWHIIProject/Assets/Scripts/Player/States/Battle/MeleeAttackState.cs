using UnityEngine;

public class MeleeAttackState : PlayerState
{

    public MeleeAttackState(PlayerManager player, PlayerStateMachine stateMachine) : base(player, stateMachine) 
    {
        if (player.comboStep == 0)
        {
            player.comboStep = 1;
        }


        player.animator.ResetTrigger("MeleeAttack" + player.comboStep);
    }



    public override void Enter()
    {

        if (player.comboStep == 0)
        {
            player.comboStep = 1;
        }

        player.animator.SetTrigger("MeleeAttack" + player.comboStep);
        player.animator.SetBool("AttackEnd", false);
    }

    public override void HandleInput()
    {
        if (player.playerInput.actions["Attack"].triggered)
        {
            // 콤보가 가능한 상태에서만 큐에 등록
            if (player.canCombo)
            {
                player.comboQueued = true;
            }
        }
    }

    public override void PsUpdate()
    {

        AnimatorStateInfo info = player.animator.GetCurrentAnimatorStateInfo(0);

        // 콤보 입력 처리
        if (player.comboQueued && player.canCombo && player.comboStep == 1)
        {
            player.comboQueued = false;
            player.comboStep = 2;
            player.animator.SetTrigger("MeleeAttack2");
            player.canCombo = false;
        }

            // 애니메이션이 끝났을 때 상태 전환
            if ((info.IsName("Attack") || info.IsName("Attack2")) && info.normalizedTime >= 1f)
            {
            player.comboStep = 1;
            player.canCombo = false;
            player.comboQueued = false;
            player.animator.SetBool("AttackEnd", true);
            stateMachine.ChageState(new LocomotionState(player, stateMachine)); // 움직이면 이동으로
            }
        }
    }
