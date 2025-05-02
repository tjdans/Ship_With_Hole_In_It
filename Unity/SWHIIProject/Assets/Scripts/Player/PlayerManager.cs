using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    //새로 개선한 코드
    //------------- 조작 외에 플레이어한테 필요한 것들 -------------
    [Header("References")]
    public CharacterController controller;
    public Animator animator;
    public PlayerInput playerInput;
    public Transform cameraTransform;

    [Header("Settings")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;
    public float rollSpeed = 8.0f;
    public float rollDuration = 1.18f;
    public float glideSpeed = 8.0f;
    public float normalGravity = -9.81f;
    public float glideGravity = -2.0f;
    public float maxGlideFallSpeed = 2.0f;

    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Vector3 moveDirection;
    [HideInInspector] public bool isGliding;
    [HideInInspector] public bool isRolling;

    private PlayerState currentState;
    private PlayerStateMachine stateMachine;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        stateMachine = new PlayerStateMachine();
    }

    private void Start()
    {
        stateMachine.Initialize(new IdleState(this, stateMachine));
    }

    private void Update()
    {
        stateMachine.SmUpdate();
    }

    // 새로 개선하기 전
    //  [Header("References")]
    //  [SerializeField] private Transform cameraTransform;

    //  //------------- 조작에 관련 부분 -------------
    //  [Header("Movement Settings")]
    //  [SerializeField] private float moveSpeed = 5.0f;
    //  [SerializeField] private float jumpForce = 5.0f;
    //  [SerializeField] private float rollSpeed = 8.0f;
    //  [SerializeField] private float rollDuration = 1.18f;
    //  [SerializeField] private float normalGravity = -9.81f;
    //  [SerializeField] private float glideGravity = -2.0f;
    //  [SerializeField] private float maxGlideFallSpeed = 2.0f;

    //  private CharacterController controller;
    //  private Animator animator;
    //  private PlayerInput playerInput;

    //  //해시화한 애니메이터 파라미터들
    //  private static readonly int JumpFromIdleHash = Animator.StringToHash("JumpFromIdle");
    //  private static readonly int JumpFromeRunHash = Animator.StringToHash("JumpFromRun");
    //  private static readonly int isRunningHash = Animator.StringToHash("isRunning");
    //  private static readonly int isGlideingHash = Animator.StringToHash("isGliding");
    //  private static readonly int RollHash = Animator.StringToHash("Roll");
    ////  private static readonly int JumpSpeedHash = Animator.StringToHash("JumpSpeed");

    //  private Vector2 moveInput;
    //  private Vector3 moveDirection;
    //  private Vector3 velocity;
    //  private Vector3 rollDirection;

    //  private float rollTimer;
    //  private bool isGliding = false;
    //  private bool isRolling = false;
    //  private bool wasMovingBeforeJump = false; // 점프 직전 이동중이었는지 기억

    //  private enum PlayerState { Idle, Run, JumpFromIdle, JumpFromRun, Glide, Roll }
    //  private PlayerState currentState = PlayerState.Idle;

    //  private void Awake()
    //  {
    //      controller = GetComponent<CharacterController>();
    //      animator = GetComponent<Animator>();
    //      playerInput = GetComponent<PlayerInput>();
    //  }

    //  private void Update()
    //  {
    //      HandleInput();
    //      HandleStateMachine();
    //      ApplyGravity();
    //      MoveCharacter();
    //   // UpdateGlideAnimation();

    //      if(controller.isGrounded &&(currentState == PlayerState.JumpFromRun || currentState == PlayerState.JumpFromIdle))
    //      {
    //          animator.SetTrigger("Land");
    //      }
    //  }

    //  private void HandleInput()
    //  {
    //      moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

    //      if (playerInput.actions["Jump"].triggered)
    //          OnJumpPressed();
    //  }

    //  private void HandleStateMachine()
    //  {
    //      switch (currentState)
    //      {
    //          case PlayerState.Idle:
    //              HandleIdle();
    //              break;
    //          case PlayerState.Run:
    //              HandleRun();
    //              break;
    //          case PlayerState.JumpFromIdle:
    //          case PlayerState.JumpFromRun:
    //              HandleJump();
    //              break;
    //          case PlayerState.Glide:
    //              HandleGlide();
    //              break;
    //          case PlayerState.Roll:
    //              HandleRoll();
    //              break;
    //      }
    //  }

    //  private void HandleIdle()
    //  {
    //      if (moveInput.magnitude > 0.1f)
    //          TransitionToRun();
    //  }

    //  private void HandleRun()
    //  {
    //      if (moveInput.magnitude < 0.1f)
    //          TransitionToIdle();
    //  }

    //  private void HandleJump()
    //  {
    //      if (controller.isGrounded && velocity.y <= 0)
    //      {
    //          // 착지 시, 점프 전에 이동하고 있었으면 달리기 복구
    //          if (wasMovingBeforeJump)
    //              TransitionToRun();
    //          else
    //              TransitionToIdle();
    //      }
    //  }

    //  private void HandleGlide()
    //  {
    //      if (controller.isGrounded)
    //      {
    //          isGliding = false;
    //          animator.SetBool(isGlideingHash, false);
    //          if (moveInput.magnitude > 0.1f)
    //              TransitionToRun();
    //          else
    //              TransitionToIdle();

    //              StartRoll(); // 활공 착지 후 구르기
    //      }
    //  }

    //  private void HandleRoll()
    //  {
    //      rollTimer -= Time.deltaTime;
    //      controller.Move(rollDirection * rollSpeed * Time.deltaTime);

    //      if (rollTimer <= 0f)
    //      {
    //          isRolling = false;
    //          if (moveInput.magnitude > 0.1f)
    //              TransitionToRun();
    //          else
    //              TransitionToIdle();
    //      }
    //  }

    //  private void MoveCharacter()
    //  {
    //      if (isRolling)
    //      {
    //          moveInput = Vector2.zero;
    //          return;
    //      }


    //      Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

    //      if (currentState == PlayerState.Glide)
    //      {
    //          // 활공 중에는 W키만 이동 허용
    //          if (moveInput.y <= 0f)
    //              return;
    //          move = new Vector3(0, 0, 1);
    //      }

    //      if (move.magnitude > 0.1f)
    //      {
    //          moveDirection = cameraTransform.forward * move.z + cameraTransform.right * move.x;
    //          moveDirection.y = 0;
    //          moveDirection.Normalize();

    //          controller.Move(moveDirection * moveSpeed * Time.deltaTime);

    //          Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
    //          transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
    //      }
    //  }

    //  private void ApplyGravity()
    //  {
    //      if (controller.isGrounded && velocity.y < 0)
    //      {
    //          velocity.y = -2f;
    //      }
    //      else
    //      {
    //          if (currentState == PlayerState.Glide)
    //          {
    //              velocity.y += glideGravity * Time.deltaTime;
    //              velocity.y = Mathf.Max(velocity.y, -maxGlideFallSpeed);
    //          }
    //          else
    //          {
    //              velocity.y += normalGravity * Time.deltaTime;
    //          }
    //      }

    //      controller.Move(velocity * Time.deltaTime);
    //  }

    //  private void OnJumpPressed()
    //  {
    //      if (currentState == PlayerState.Idle || currentState == PlayerState.Run)
    //      {
    //          `();
    //      }
    //      else if ((currentState == PlayerState.JumpFromIdle || currentState == PlayerState.JumpFromRun) && !isGliding)
    //      {
    //          StartGlide();
    //      }
    //      else if (currentState == PlayerState.Glide && isGliding)
    //      {
    //          StopGlideAndJumpAgain(); // 활공 중 다시 점프
    //      }
    //  }

    //  private void JumpFromState()
    //  {
    //      if(currentState == PlayerState.Idle)
    //      {
    //          wasMovingBeforeJump = false; // 정지 상태에서 점프
    //          velocity.y = Mathf.Sqrt(jumpForce * -2f * normalGravity);
    //          animator.SetTrigger(JumpFromIdleHash);
    //          currentState = PlayerState.JumpFromIdle;
    //      }
    //      else
    //      {
    //          wasMovingBeforeJump = true; // 이동 중 점프
    //          velocity.y = Mathf.Sqrt(jumpForce * -2f * normalGravity);
    //          animator.SetTrigger(JumpFromeRunHash);
    //          currentState = PlayerState.JumpFromRun;
    //      }
    //  }

    //  private void StartGlide()
    //  {
    //      isGliding = true;
    //      currentState = PlayerState.Glide;
    //      animator.SetBool(isGlideingHash, true);
    //  }

    //  private void StopGlideAndJumpAgain()
    //  {
    //      isGliding = false;
    //      animator.SetBool(isGlideingHash, false);

    //      // 활공 중 다시 점프할 때, 점프 종류를 이전 이동 상태에 따라 나눔
    //      if (wasMovingBeforeJump)
    //      {
    //          animator.SetTrigger(JumpFromeRunHash);
    //          currentState = PlayerState.JumpFromRun;
    //      }
    //      else
    //      {
    //          animator.SetTrigger(JumpFromIdleHash);
    //          currentState = PlayerState.JumpFromIdle;
    //      }
    //  }

    //  private void StartRoll()
    //  {
    //      isRolling = true;
    //      rollTimer = rollDuration;
    //      rollDirection = transform.forward;
    //      currentState = PlayerState.Roll;
    //      animator.SetTrigger(RollHash);
    //  }

    //  private void TransitionToIdle()
    //  {
    //      currentState = PlayerState.Idle;
    //      animator.SetBool(isRunningHash, false);
    //  }

    //  private void TransitionToRun()
    //  {
    //      currentState = PlayerState.Run;
    //      animator.SetBool(isRunningHash, true);
    //  }

    //private void UpdateJumpAnimationSpeed()
    //{
    //    if (currentState == PlayerState.JumpFromIdle || currentState == PlayerState.JumpFromRun)
    //    {
    //        float fallSpeed = Mathf.Abs(velocity.y * Time.deltaTime);
    //        float speedMultiplier = Mathf.Lerp(1f, 2f, fallSpeed / 10f);
    //        animator.SetFloat(JumpSpeedHash, speedMultiplier);
    //    }
    //}

    //private void UpdateGlideAnimation()
    //{
    //    if(currentState == PlayerState.Glide)
    //    {
    //        float glideBlend = Mathf.InverseLerp(-maxGlideFallSpeed, 0f, velocity.y);
    //        animator.SetFloat("GlideBlend", glideBlend);
    //    }
    //}
} 