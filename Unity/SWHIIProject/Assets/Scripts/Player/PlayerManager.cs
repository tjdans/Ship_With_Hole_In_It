using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    //------------- 조작 외에 플레이어한테 필요한 것들 -------------
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    //------------- 조작에 관련 부분 -------------
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
    private bool wasMovingBeforeJump = false; // 점프 직전 이동중이었는지 기억

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
            // 착지 시, 점프 전에 이동하고 있었으면 달리기 복구
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
            StartRoll(); // 활공 착지 후 구르기
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
            // 활공 중에는 W키만 이동 허용
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
            StopGlideAndJumpAgain(); // 활공 중 다시 점프
        }
    }

    private void JumpFromIdle()
    {
        wasMovingBeforeJump = false; // 정지 상태에서 점프
        velocity.y = Mathf.Sqrt(jumpForce * -2f * normalGravity);
        animator.SetTrigger("JumpFromIdle");
        currentState = PlayerState.JumpFromIdle;
    }

    private void JumpFromRun()
    {
        wasMovingBeforeJump = true; // 이동 중 점프
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

        // 활공 중 다시 점프할 때, 점프 종류를 이전 이동 상태에 따라 나눔
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

    //플레이어 상태이상효과
    public void CharacterStat()
    {
        //탈진상태인 경우에서 일단 스테미너 회복 최대치까지 안됬을경우
        if (player.Sit.HasFlag(PlayerStat.situation.exhaustion))
        {
            //안되는 행동들 코드적기
        }
        //플레이어 배고픔수치가 50%이하일 경우 채력재생 정지
        if (player.Sit.HasFlag(PlayerStat.situation.smallhunger))
        {
            player.Hp += player.Hpregeneration * 0;
            player.HungryStat -= (int)Time.deltaTime;
        }
        //배고파서 채력 떨어지는경우(피격당할때 hp감소되는건 따로 함수쓰는게 나을듯? 싶어서? ㅇㅇ)
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
        //무겁데
        if (player.Sit.HasFlag(PlayerStat.situation.haviness))
        {
            //무게를 초과하여 들 경우 2배로 배고픔,수분수치 감소 + 이속 50%로 감소
            player.HungryStat -= player.Hpregeneration * 2 * (int)Time.time;
            player.ThirstyStat -= player.Staminaregeneration * 2 * (int)Time.time;
            moveSpeed = 2.5f; // 일단 숫자로 함 아직 뭐 더 안나왔으니
        }
        else
        {
            player.HungryStat -= player.Hpregeneration * (int)Time.time;
            player.ThirstyStat -= player.Staminaregeneration * (int)Time.time;
            moveSpeed = 5.0f;
        }
        //변수명 고치기 귀찮아서 내일고침 목마름수치 0되면 스테미너 회복정지
        //플레이어 목마름수치가 50%이하일 경우 스테미너 회복 속도 감소
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