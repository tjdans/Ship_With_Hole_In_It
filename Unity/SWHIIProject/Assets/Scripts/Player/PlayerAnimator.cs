using UnityEngine;

// 이 스크립트는 해시화 + 플레이어 애니메이션 처리 담당 
//지금 하기에는 이른 것 같아서 조작 부분 어느정도 구현되면 처리할게
public class PlayerAnimator
{
    private Animator _animator;
    private PlayerManager _player;

    private readonly int _motionSpeedAnim = Animator.StringToHash("Speed");
    private readonly int _idleAnim = Animator.StringToHash("Idle");
    private readonly int _runAnim = Animator.StringToHash("Run");
    private readonly int _walkAnim = Animator.StringToHash("Walk");
    private readonly int _normalJumpAnim = Animator.StringToHash("NormalJump");
    private readonly int _runJumpAnim = Animator.StringToHash("RunJump");
    private readonly int _glideAnim = Animator.StringToHash("Glide");
    private readonly int _attackAnim = Animator.StringToHash("Attack");



}
