using UnityEngine;
using UnityEngine.InputSystem;

//이 클래스는 오로지 플레이어의 이동,점프,공격, 중력처리하는 클래스임
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rollDistance = 10.0f;
    [SerializeField] private float rollDuration = 1.18f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpForce = 2.0f;
    //실제 지구 표면에서 작용하는 중력 가속도 값 9.81m/s를 기반
    [SerializeField] private float normalGravity = -9.81f;
    [SerializeField] private float glideGravity = -2.0f;
    [SerializeField] private float maxGlideFallSpeed = 2.0f;

    [Header("References")]
    [SerializeField] private Transform camerTransform;

    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Roll,
        Gliding,
        Attak,
        Die
    }

    private CharacterController controller;

    private Vector2 moveInput;
    private Vector3 rollInput;
    private Vector3 velocity;

    private bool isJumpPressed = false; // 점프키를 눌렀는지 판단하는 변수
    private bool isRolling = false;
    private bool isGliding = false;
    private bool hasJump = false;// 점프 중인지를 판단하는 변수

    private float rollTimer = 0f;

    private Animator animator;
    private PlayerState currentState = PlayerState.Idle;

<<<<<<< HEAD
    PlayerStat player = new();
    private bool wasGrounded = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        player = gameObject.AddComponent<PlayerStat>();
    }

    private void Update()
     {
        if(isRolling)
        {

            HandleRoll();
        }
        else
        {
            HandleMovement();
            HandleJump();
            ApplyGravity();
        }
        DetectLanding(); // 활공 후 착지할 때 구르기가 시작되게 하는 함수 그냥 점프문에 넣으니깐 작동 안되서 엄청 헤매다 결국 여기에 넣음 Tㅂ
        UpdateState();
        UpdateAnimator();
        wasGrounded = controller.isGrounded; // 활공 후 착지를 하는데 오류로 지면에 닿아도 착지가 안되서 넣음 Fuck
    }
    
    
    // ------------Input System Callback 함수들------------------
    // New Input System에서 'Move' 액션이 호출되었을 때 실행됨
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // 'Jump' 액션이 호출될 때 실행됨
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;

        if (controller.isGrounded)
        {
            isJumpPressed = true;
        }
        else if (isGliding)
        {
            // 활공 중일 때 스페이스바 누르면 활공 종료
            isGliding = false;
            hasJump = false;
            currentState = PlayerState.Jump;
            UpdateAnimator(); // 즉시 애니메이터 업데이트
            Debug.Log("[활공 중 취소 → 점프 상태로]");

        }
        else if (!isGliding && hasJump)
        {
            // 점프 후 공중일 때만 활공 시작
            isGliding = true;
            Debug.Log("[활공 시작]");

        }
    }
    //구르기 호출
    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && !isRolling)
        {
            StartRoll();
        }
    }

    // 'Attack' 액션 예시용 함수 (현재는 단순 로그 출력)
    public void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack");
    }

    private void Update()
     {
        if(isRolling)
        {
            HandleRoll();
        }
        else
        {
            HandleMovement();
            HandleJump();
            ApplyGravity();
        }

        UpdateState();
        UpdateAnimator();
        CharacterStat();
    }

    //--------------플레이어 조작 관련 함수들-------------------------
    // 입력 벡터에 따라 이동 처리
    private void HandleMovement()
    {
        //활공 상태에서 W키만 이동 허용하는 부분
        if(currentState == PlayerState.Gliding)
        {
            // W키가 아니면 return으로 이동 무시
            if(moveInput.y <= 0.0f)
            {
                return;
            }
        }

        // X와 Y 값을 3D 방향으로 변환 (Z축이 전진 방향)
        Vector3 move = new Vector3(moveInput.x, 0.0f, moveInput.y);

        //카메라의 정보가 있다면
        if(camerTransform != null)
        {
            Vector3 cameraForward = camerTransform.forward;
            Vector3 cameraRight = camerTransform.right;
            cameraForward.y = 0.0f;
            cameraRight.y = 0.0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 disiredMove = cameraForward * move.z + cameraRight * move.x;
            if (disiredMove.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(disiredMove);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 30.0f);
            }
            controller.Move(disiredMove * moveSpeed*Time.deltaTime);
        }
        else
        {
            //캐릭터의 전방 방향 기준으로 변환
            move = transform.TransformDirection(move);
            // 캐릭터 컨트롤러 컴포넌트를 통해 이동 적용
            controller.Move(move * moveSpeed * Time.deltaTime);
        }
    }

    private void DetectLanding()
    {
        if (!wasGrounded && controller.isGrounded && isGliding)
        {
            Debug.Log("[DetectLanding] 활강 후 착지 - 구르기 시작");
            isGliding = false;
            StartRoll();
        }
    }

    // 점프 처리
    private void HandleJump()
    {
        if (controller.isGrounded)
        {
            isGliding = false; // 여기에 미리 false
            velocity.y = -2f;
            hasJump = false;
        }

        if (isJumpPressed)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * normalGravity);
            isJumpPressed = false;
            hasJump = true;
            //달리기 점프와 대기 점프 구분 부분
           if(currentState == PlayerState.Run)
            {
                animator.ResetTrigger("isIdleJump");
                animator.SetTrigger("isRunJump");
            }
            else
            {
                animator.ResetTrigger("isRunJump");
                animator.SetTrigger("isIdleJump");
            }
        }
    }

    //구르기 시작할때
    private void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;
        currentState = PlayerState.Roll;

        rollInput = transform.forward;

       animator.SetTrigger("isRoll");
    }

    //구르기 이동함수
    private void HandleRoll()
    {
        controller.Move(rollInput * rollDistance * Time.deltaTime);
        rollTimer -= Time.deltaTime;

        if(rollTimer <= 0.0f)
        {
            isRolling = false;

            if(moveInput.sqrMagnitude > 0.1f)
            {
                currentState = PlayerState.Run;
            }
            else
            {
                currentState = PlayerState.Idle;
            }

        }
    }

    // 중력을 매 프레임 적용하는 함수
    private void ApplyGravity()
    {
        float gravity = isGliding ? glideGravity : normalGravity;
        velocity.y += gravity * Time.deltaTime;

        // 최종적으로 중력에 의해 바뀐 속도를 이동에 적용
        controller.Move(velocity * Time.deltaTime);

        if(isGliding && velocity.y < -maxGlideFallSpeed)
        {
            velocity.y = -maxGlideFallSpeed;
        }

    }

    //플레이어 상태 진단, 변경 함수
    private void UpdateState()
    {
        // 구르는 상태는 상태 변경 없음 구르기가 끝나면 false로 변경되서 구르고 난 후에 상태를 진단하게 함
        if (isRolling) return;

        else if(!controller.isGrounded)
        {
            if (isGliding) 
                currentState = PlayerState.Gliding;
            else if(hasJump)
                currentState = PlayerState.Jump;
        }
       else if(controller.isGrounded && moveInput.sqrMagnitude > 0.1f)
        {
            currentState = PlayerState.Run;
        }
       else
        {
            currentState = PlayerState.Idle;
        }
    }

    //플레이어 애니메이터 파라미터 변경 함수
    //공격파라미터 변경은 OnAttack함수에
    private void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetBool("isRunning", currentState == PlayerState.Run);
        animator.SetBool("isGliding", currentState == PlayerState.Gliding);
        animator.SetFloat("VerticalSpeed", velocity.y);
    }

    //플레이어 상태이상효과
    public void CharacterStat()
    {
        //탈진상태인 경우에서 일단 스테미너 회복 최대치까지 안됬을경우
        if (player.Sit.HasFlag(PlayerStat.situation.exhaustion))
        {
            //안되는 행동들 코드적기
        }
        //배고파서 채력 떨어지는경우(피격당할때 hp감소되는건 따로 함수쓰는게 나을듯? 싶어서? ㅇㅇ)
        if (player.Sit.HasFlag(PlayerStat.situation.hunger))
        {
            player.Hp -= (int)Time.time;
        }
        //배고픈상태 아닌경우(PlayerStat코드)
        else
        {
            player.Hp += (int)Time.time;
        }
        //무겁데
        if (player.Sit.HasFlag(PlayerStat.situation.haviness))
        {
            //일단 2배깍이는거
            player.HungryStat -= player.Hpregeneration * 2 * (int)Time.time;
            player.ThirstyStat -= player.Staminaregeneration * 2 * (int)Time.time;
        }
        else
        {
            player.HungryStat -= player.Hpregeneration * (int)Time.time;
            player.ThirstyStat -= player.Staminaregeneration * (int)Time.time;
        }
        //변수명 고치기 귀찮아서 내일고침 목마름수치 0되면 스테미너 회복정지
        if (player.Sit.HasFlag(PlayerStat.situation.thirst))
        {
            player.Stamina += 0;
        }
        else
        {
            player.Stamina += (int)Time.time;
        }
    }
  }
}