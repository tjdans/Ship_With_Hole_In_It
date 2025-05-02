using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    //------------- ���� �ܿ� �÷��̾����� �ʿ��� �͵� -------------
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    //------------- ���ۿ� ���� �κ� -------------
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float rollSpeed = 8.0f;
    [SerializeField] private float rollDuration = 1.18f;
    [SerializeField] private float normalGravity = -9.81f;
    [SerializeField] private float glideGravity = -2.0f;
    [SerializeField] private float maxGlideFallSpeed = 2.0f;

    private CharacterController controller;
    private Animator animator;
    private PlayerInput playerInput;

    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 velocity;
    private Vector3 rollDirection;

    private float rollTimer;
    private bool isGliding = false;
    private bool isRolling = false;
    private bool wasMovingBeforeJump = false; // ���� ���� �̵����̾����� ���

    private enum PlayerState { Idle, Run, JumpFromIdle, JumpFromRun, Glide, Roll }
    private PlayerState currentState = PlayerState.Idle;

    public PlayerStat player;
    private void Awake()
    {
        player = GetComponent<PlayerStat>();
        player = new PlayerStat();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        HandleInput();
        HandleStateMachine();
        ApplyGravity();
        MoveCharacter();
        UpdateJumpAnimationSpeed();
        CharacterStat();
    }

    private void HandleInput()
    {
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        if (playerInput.actions["Jump"].triggered)
            OnJumpPressed();
    }

    private void HandleStateMachine()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                HandleIdle();
                break;
            case PlayerState.Run:
                HandleRun();
                break;
            case PlayerState.JumpFromIdle:
            case PlayerState.JumpFromRun:
                HandleJump();
                break;
            case PlayerState.Glide:
                HandleGlide();
                break;
            case PlayerState.Roll:
                HandleRoll();
                break;
        }
    }

    private void HandleIdle()
    {
        if (moveInput.magnitude > 0.1f)
            TransitionToRun();
    }

    private void HandleRun()
    {
        if (moveInput.magnitude < 0.1f)
            TransitionToIdle();
    }

    private void HandleJump()
    {
        if (controller.isGrounded && velocity.y <= 0)
        {
            // ���� ��, ���� ���� �̵��ϰ� �־����� �޸��� ����
            if (wasMovingBeforeJump)
                TransitionToRun();
            else
                TransitionToIdle();
        }
    }

    private void HandleGlide()
    {
        if (controller.isGrounded)
        {
            isGliding = false;
            animator.SetBool("isGliding", false);
            StartRoll(); // Ȱ�� ���� �� ������
        }
    }

    private void HandleRoll()
    {
        rollTimer -= Time.deltaTime;
        controller.Move(rollDirection * rollSpeed * Time.deltaTime);

        if (rollTimer <= 0f)
        {
            isRolling = false;
            if (moveInput.magnitude > 0.1f)
                TransitionToRun();
            else
                TransitionToIdle();
        }
    }

    private void MoveCharacter()
    {
        if (isRolling)
            return;

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        if (currentState == PlayerState.Glide)
        {
            // Ȱ�� �߿��� WŰ�� �̵� ���
            if (moveInput.y <= 0f)
                return;
            move = new Vector3(0, 0, 1);
        }

        if (move.magnitude > 0.1f)
        {
            moveDirection = cameraTransform.forward * move.z + cameraTransform.right * move.x;
            moveDirection.y = 0;
            moveDirection.Normalize();

            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            if (currentState == PlayerState.Glide)
            {
                velocity.y += glideGravity * Time.deltaTime;
                velocity.y = Mathf.Max(velocity.y, -maxGlideFallSpeed);
            }
            else
            {
                velocity.y += normalGravity * Time.deltaTime;
            }
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void OnJumpPressed()
    {
        if (currentState == PlayerState.Idle)
        {
            JumpFromIdle();
        }
        else if (currentState == PlayerState.Run)
        {
            JumpFromRun();
        }
        else if ((currentState == PlayerState.JumpFromIdle || currentState == PlayerState.JumpFromRun) && !isGliding)
        {
            StartGlide();
        }
        else if (currentState == PlayerState.Glide && isGliding)
        {
            StopGlideAndJumpAgain(); // Ȱ�� �� �ٽ� ����
        }
    }

    private void JumpFromIdle()
    {
        wasMovingBeforeJump = false; // ���� ���¿��� ����
        velocity.y = Mathf.Sqrt(jumpForce * -2f * normalGravity);
        animator.SetTrigger("JumpFromIdle");
        currentState = PlayerState.JumpFromIdle;
    }

    private void JumpFromRun()
    {
        wasMovingBeforeJump = true; // �̵� �� ����
        velocity.y = Mathf.Sqrt(jumpForce * -2f * normalGravity);
        animator.SetTrigger("JumpFromRun");
        currentState = PlayerState.JumpFromRun;
    }

    private void StartGlide()
    {
        isGliding = true;
        currentState = PlayerState.Glide;
        animator.SetBool("isGliding", true);
    }

    private void StopGlideAndJumpAgain()
    {
        isGliding = false;
        animator.SetBool("isGliding", false);

        // Ȱ�� �� �ٽ� ������ ��, ���� ������ ���� �̵� ���¿� ���� ����
        if (wasMovingBeforeJump)
        {
            animator.SetTrigger("JumpFromRun");
            currentState = PlayerState.JumpFromRun;
        }
        else
        {
            animator.SetTrigger("JumpFromIdle");
            currentState = PlayerState.JumpFromIdle;
        }
    }

    private void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;
        rollDirection = transform.forward;
        currentState = PlayerState.Roll;
        animator.SetTrigger("Roll");
    }

    private void TransitionToIdle()
    {
        currentState = PlayerState.Idle;
        animator.SetBool("isRunning", false);
    }

    private void TransitionToRun()
    {
        currentState = PlayerState.Run;
        animator.SetBool("isRunning", true);
    }

    private void UpdateJumpAnimationSpeed()
    {
        if (currentState == PlayerState.JumpFromIdle || currentState == PlayerState.JumpFromRun)
        {
            float fallSpeed = Mathf.Abs(velocity.y * Time.deltaTime);
            float speedMultiplier = Mathf.Lerp(1f, 2f, fallSpeed / 10f);
            animator.SetFloat("JumpSpeed", speedMultiplier);
        }
    }

    //�÷��̾� �����̻�ȿ��
    public void CharacterStat()
    {
        //Ż�������� ��쿡�� �ϴ� ���׹̳� ȸ�� �ִ�ġ���� �ȉ������
        if (player.Sit.HasFlag(PlayerStat.situation.exhaustion))
        {
            //�ȵǴ� �ൿ�� �ڵ�����
        }
        //�÷��̾� ����ļ�ġ�� 50%������ ��� ä����� ����
        if (player.Sit.HasFlag(PlayerStat.situation.smallhunger))
        {
            player.Hp += player.Hpregeneration * 0;
            player.HungryStat -= (int)Time.deltaTime;
        }
        //����ļ� ä�� �������°��(�ǰݴ��Ҷ� hp���ҵǴ°� ���� �Լ����°� ������? �;? ����)
        else if (player.Sit.HasFlag(PlayerStat.situation.hunger))
        {
            player.Hp -= (int)Time.time;
            player.HungryStat -= 0;
        }
        else
        {
            player.Hp += player.Hpregeneration * (int)Time.time;
            player.HungryStat -= (int)Time.deltaTime;
        }
        //���̵�
        if (player.Sit.HasFlag(PlayerStat.situation.haviness))
        {
            //���Ը� �ʰ��Ͽ� �� ��� 2��� �����,���м�ġ ���� + �̼� 50%�� ����
            player.HungryStat -= player.Hpregeneration * 2 * (int)Time.time;
            player.ThirstyStat -= player.Staminaregeneration * 2 * (int)Time.time;
            moveSpeed = 2.5f; // �ϴ� ���ڷ� �� ���� �� �� �ȳ�������
        }
        else
        {
            player.HungryStat -= player.Hpregeneration * (int)Time.time;
            player.ThirstyStat -= player.Staminaregeneration * (int)Time.time;
            moveSpeed = 5.0f;
        }
        //������ ��ġ�� �����Ƽ� ���ϰ�ħ �񸶸���ġ 0�Ǹ� ���׹̳� ȸ������
        //�÷��̾� �񸶸���ġ�� 50%������ ��� ���׹̳� ȸ�� �ӵ� ����
        if (player.Sit.HasFlag(PlayerStat.situation.smallhunger))
        {
            player.Stamina += player.Staminaregeneration * (int)Time.time / 2;
        }
        if (player.Sit.HasFlag(PlayerStat.situation.thirst))
        {
            player.Stamina += player.Staminaregeneration * 0;
        }
        else
        {
            player.Stamina += player.Staminaregeneration * (int)Time.time;
        }
        if (isGliding == true)
        {
            player.GlidingStat -= Time.deltaTime * 2f;
            Debug.Log(player.GlidingStat);
            if (player.GlidingStat <= 0)
            {
                StopGlideAndJumpAgain();
            }
        }
        else
        {
            player.GlidingStat += Time.deltaTime;
            Debug.Log(player.GlidingStat);
        }
    }
}