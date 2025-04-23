using UnityEngine;
using UnityEngine.InputSystem;

//�� Ŭ������ ������ �÷��̾��� �̵�,����,����, �߷�ó���ϴ� Ŭ������
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rollDistance = 10.0f;
    [SerializeField] private float rollDuration = 1.18f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpForce = 2.0f;
    //���� ���� ǥ�鿡�� �ۿ��ϴ� �߷� ���ӵ� �� 9.81m/s�� ���
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

    private bool isJumpPressed = false; // ����Ű�� �������� �Ǵ��ϴ� ����
    private bool isRolling = false;
    private bool isGliding = false;
    private bool hasJump = false;// ���� �������� �Ǵ��ϴ� ����

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
        DetectLanding(); // Ȱ�� �� ������ �� �����Ⱑ ���۵ǰ� �ϴ� �Լ� �׳� �������� �����ϱ� �۵� �ȵǼ� ��û ��Ŵ� �ᱹ ���⿡ ���� T��
        UpdateState();
        UpdateAnimator();
        wasGrounded = controller.isGrounded; // Ȱ�� �� ������ �ϴµ� ������ ���鿡 ��Ƶ� ������ �ȵǼ� ���� Fuck
    }
    
    
    // ------------Input System Callback �Լ���------------------
    // New Input System���� 'Move' �׼��� ȣ��Ǿ��� �� �����
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // 'Jump' �׼��� ȣ��� �� �����
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;

        if (controller.isGrounded)
        {
            isJumpPressed = true;
        }
        else if (isGliding)
        {
            // Ȱ�� ���� �� �����̽��� ������ Ȱ�� ����
            isGliding = false;
            hasJump = false;
            currentState = PlayerState.Jump;
            UpdateAnimator(); // ��� �ִϸ����� ������Ʈ
            Debug.Log("[Ȱ�� �� ��� �� ���� ���·�]");

        }
        else if (!isGliding && hasJump)
        {
            // ���� �� ������ ���� Ȱ�� ����
            isGliding = true;
            Debug.Log("[Ȱ�� ����]");

        }
    }
    //������ ȣ��
    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && !isRolling)
        {
            StartRoll();
        }
    }

    // 'Attack' �׼� ���ÿ� �Լ� (����� �ܼ� �α� ���)
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

    //--------------�÷��̾� ���� ���� �Լ���-------------------------
    // �Է� ���Ϳ� ���� �̵� ó��
    private void HandleMovement()
    {
        //Ȱ�� ���¿��� WŰ�� �̵� ����ϴ� �κ�
        if(currentState == PlayerState.Gliding)
        {
            // WŰ�� �ƴϸ� return���� �̵� ����
            if(moveInput.y <= 0.0f)
            {
                return;
            }
        }

        // X�� Y ���� 3D �������� ��ȯ (Z���� ���� ����)
        Vector3 move = new Vector3(moveInput.x, 0.0f, moveInput.y);

        //ī�޶��� ������ �ִٸ�
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
            //ĳ������ ���� ���� �������� ��ȯ
            move = transform.TransformDirection(move);
            // ĳ���� ��Ʈ�ѷ� ������Ʈ�� ���� �̵� ����
            controller.Move(move * moveSpeed * Time.deltaTime);
        }
    }

    private void DetectLanding()
    {
        if (!wasGrounded && controller.isGrounded && isGliding)
        {
            Debug.Log("[DetectLanding] Ȱ�� �� ���� - ������ ����");
            isGliding = false;
            StartRoll();
        }
    }

    // ���� ó��
    private void HandleJump()
    {
        if (controller.isGrounded)
        {
            isGliding = false; // ���⿡ �̸� false
            velocity.y = -2f;
            hasJump = false;
        }

        if (isJumpPressed)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * normalGravity);
            isJumpPressed = false;
            hasJump = true;
            //�޸��� ������ ��� ���� ���� �κ�
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

    //������ �����Ҷ�
    private void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;
        currentState = PlayerState.Roll;

        rollInput = transform.forward;

       animator.SetTrigger("isRoll");
    }

    //������ �̵��Լ�
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

    // �߷��� �� ������ �����ϴ� �Լ�
    private void ApplyGravity()
    {
        float gravity = isGliding ? glideGravity : normalGravity;
        velocity.y += gravity * Time.deltaTime;

        // ���������� �߷¿� ���� �ٲ� �ӵ��� �̵��� ����
        controller.Move(velocity * Time.deltaTime);

        if(isGliding && velocity.y < -maxGlideFallSpeed)
        {
            velocity.y = -maxGlideFallSpeed;
        }

    }

    //�÷��̾� ���� ����, ���� �Լ�
    private void UpdateState()
    {
        // ������ ���´� ���� ���� ���� �����Ⱑ ������ false�� ����Ǽ� ������ �� �Ŀ� ���¸� �����ϰ� ��
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

    //�÷��̾� �ִϸ����� �Ķ���� ���� �Լ�
    //�����Ķ���� ������ OnAttack�Լ���
    private void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetBool("isRunning", currentState == PlayerState.Run);
        animator.SetBool("isGliding", currentState == PlayerState.Gliding);
        animator.SetFloat("VerticalSpeed", velocity.y);
    }

    //�÷��̾� �����̻�ȿ��
    public void CharacterStat()
    {
        //Ż�������� ��쿡�� �ϴ� ���׹̳� ȸ�� �ִ�ġ���� �ȉ������
        if (player.Sit.HasFlag(PlayerStat.situation.exhaustion))
        {
            //�ȵǴ� �ൿ�� �ڵ�����
        }
        //����ļ� ä�� �������°��(�ǰݴ��Ҷ� hp���ҵǴ°� ���� �Լ����°� ������? �;? ����)
        if (player.Sit.HasFlag(PlayerStat.situation.hunger))
        {
            player.Hp -= (int)Time.time;
        }
        //����»��� �ƴѰ��(PlayerStat�ڵ�)
        else
        {
            player.Hp += (int)Time.time;
        }
        //���̵�
        if (player.Sit.HasFlag(PlayerStat.situation.haviness))
        {
            //�ϴ� 2����̴°�
            player.HungryStat -= player.Hpregeneration * 2 * (int)Time.time;
            player.ThirstyStat -= player.Staminaregeneration * 2 * (int)Time.time;
        }
        else
        {
            player.HungryStat -= player.Hpregeneration * (int)Time.time;
            player.ThirstyStat -= player.Staminaregeneration * (int)Time.time;
        }
        //������ ��ġ�� �����Ƽ� ���ϰ�ħ �񸶸���ġ 0�Ǹ� ���׹̳� ȸ������
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