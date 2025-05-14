using UnityEngine;

// �� ��ũ��Ʈ�� �ؽ�ȭ + �÷��̾� �ִϸ��̼� ó�� ��� 
//���� �ϱ⿡�� �̸� �� ���Ƽ� ���� �κ� ������� �����Ǹ� ó���Ұ�
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
