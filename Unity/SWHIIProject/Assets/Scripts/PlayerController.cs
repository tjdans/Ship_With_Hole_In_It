using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

//이 클래스는 오로지 플레이어의 이동,점프,공격, 중력처리하는 클래스임
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rollSpeed = 5.0f;
    [SerializeField] private float rollDuration = 0.5f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpForce = 2.0f;
    //실제 지구 표면에서 작용하는 중력 가속도 값 9.81m/s를 기반
    [SerializeField] private float gravity = -9.81f;

    [Header("References")]
    [SerializeField] private Transform camerTransform;

    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Roll,
        Glieding,
        Attak,
        Die
    }

    private CharacterController controller;

    private Vector2 moveInput;

    private Vector3 rollInput;
    private Vector3 velocity;

    private bool isJumpPressed;
    private bool isRolling = false;

    private float rollTimer = 0f;

    private Animator animator;
    private PlayerState currentState = PlayerState.Idle;

    PlayerStat player = new();
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        player = gameObject.AddComponent<PlayerStat>();
    }


    // New Input System에서 'Move' 액션이 호출되었을 때 실행됨
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // 'Jump' 액션이 호출될 때 실행됨
    public void OnJump(InputAction.CallbackContext context)
    {
        // performed 단계에서만 점프 처리, 그리고 캐릭터가 땅에 있어야 함
        if (context.phase == InputActionPhase.Performed && controller.isGrounded)
        {
            // 점프 플래그 설정
            isJumpPressed = true;
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed && !isRolling)
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

    // 입력 벡터에 따라 이동 처리
    private void HandleMovement()
    {
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

    // 점프 처리
    private void HandleJump()
    {
        // 땅에 닿아있고 낙하 중이라면, y 속도를 작게 고정 (지면에 붙어 있게 함)
        if (controller.isGrounded && velocity.y < 0)
        {
            // 살짝 눌러붙는 느낌을 주기 위함
            velocity.y = -2f;
        }
        // 점프 입력이 눌린 경우
        if (isJumpPressed)
        {
            // 점프 힘을 중력에 맞춰 계산한 속도로 y값 적용
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            // 점프 플래그 리셋으로 중복 점프 방지
            isJumpPressed = false;
           
        }
    }

    private void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;
        currentState = PlayerState.Roll;

        rollInput = transform.forward;

        animator.SetTrigger("roll");
    }

    private void HandleRoll()
    {
        controller.Move(rollInput * rollSpeed * Time.deltaTime);
        rollTimer -= Time.deltaTime;

        if(rollTimer <= 0.0f)
        {
            isRolling = false;
            if(!controller.isGrounded)
            {
                currentState = PlayerState.Jump;
            }
            else if(moveInput.sqrMagnitude > 0.1f)
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
        // 중력 방향으로 y 속도 증가 (중력은 음수이므로 계속 하강)
        velocity.y += gravity * Time.deltaTime;
        // 최종적으로 중력에 의해 바뀐 속도를 이동에 적용
        controller.Move(velocity * Time.deltaTime);

    }

    //플레이어 상태 진단, 변경 함수
    private void UpdateState()
    {
       if(!controller.isGrounded)
        {
            currentState = PlayerState.Jump;
        }
       else if(controller.isGrounded && moveInput.sqrMagnitude > 0.1f)
        {
            currentState = PlayerState.Run;
        }
       else if(rollInput.sqrMagnitude > 0.1f)
        {
            currentState = PlayerState.Roll;
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
        animator.SetBool("isRunning", currentState == PlayerState.Run);
        animator.SetBool("isJumpping", currentState == PlayerState.Jump);
        animator.SetBool("isRolling", currentState == PlayerState.Roll);
    }

    //플레이어 상태이상효과
    public void CharacterStat()
    {
        //탈진상태인 경우에서 일단 스테미너 회복 최대치까지 안됬을경우
        if (player.Sit.HasFlag(PlayerStat.situation.exhaustion))
        {
            //안되는 행동들 코드적기
        }
        //탈진상태 제거시(PlayerStat코드)
        else
        {
            player.Stamina += (int)Time.time;
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
            //안되는 행동들 적기
        }
        //변수명 고치기 귀찮아서 내일고침 목마름수치 0되면 스테미너 회복정지
        if (player.Sit.HasFlag(PlayerStat.situation.thirst))
        {
            player.Stamina += 0;
        }
    }

}
