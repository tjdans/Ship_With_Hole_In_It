using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

//�� Ŭ������ ������ �÷��̾��� �̵�,����,����, �߷�ó���ϴ� Ŭ������
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rollSpeed = 5.0f;
    [SerializeField] private float rollDuration = 0.5f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpForce = 2.0f;
    //���� ���� ǥ�鿡�� �ۿ��ϴ� �߷� ���ӵ� �� 9.81m/s�� ���
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


    // New Input System���� 'Move' �׼��� ȣ��Ǿ��� �� �����
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // 'Jump' �׼��� ȣ��� �� �����
    public void OnJump(InputAction.CallbackContext context)
    {
        // performed �ܰ迡���� ���� ó��, �׸��� ĳ���Ͱ� ���� �־�� ��
        if (context.phase == InputActionPhase.Performed && controller.isGrounded)
        {
            // ���� �÷��� ����
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

    // �Է� ���Ϳ� ���� �̵� ó��
    private void HandleMovement()
    {
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

    // ���� ó��
    private void HandleJump()
    {
        // ���� ����ְ� ���� ���̶��, y �ӵ��� �۰� ���� (���鿡 �پ� �ְ� ��)
        if (controller.isGrounded && velocity.y < 0)
        {
            // ��¦ �����ٴ� ������ �ֱ� ����
            velocity.y = -2f;
        }
        // ���� �Է��� ���� ���
        if (isJumpPressed)
        {
            // ���� ���� �߷¿� ���� ����� �ӵ��� y�� ����
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            // ���� �÷��� �������� �ߺ� ���� ����
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

    // �߷��� �� ������ �����ϴ� �Լ�
    private void ApplyGravity()
    {
        // �߷� �������� y �ӵ� ���� (�߷��� �����̹Ƿ� ��� �ϰ�)
        velocity.y += gravity * Time.deltaTime;
        // ���������� �߷¿� ���� �ٲ� �ӵ��� �̵��� ����
        controller.Move(velocity * Time.deltaTime);

    }

    //�÷��̾� ���� ����, ���� �Լ�
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

    //�÷��̾� �ִϸ����� �Ķ���� ���� �Լ�
    //�����Ķ���� ������ OnAttack�Լ���
    private void UpdateAnimator()
    {
        animator.SetBool("isRunning", currentState == PlayerState.Run);
        animator.SetBool("isJumpping", currentState == PlayerState.Jump);
        animator.SetBool("isRolling", currentState == PlayerState.Roll);
    }

    //�÷��̾� �����̻�ȿ��
    public void CharacterStat()
    {
        //Ż�������� ��쿡�� �ϴ� ���׹̳� ȸ�� �ִ�ġ���� �ȉ������
        if (player.Sit.HasFlag(PlayerStat.situation.exhaustion))
        {
            //�ȵǴ� �ൿ�� �ڵ�����
        }
        //Ż������ ���Ž�(PlayerStat�ڵ�)
        else
        {
            player.Stamina += (int)Time.time;
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
            //�ȵǴ� �ൿ�� ����
        }
        //������ ��ġ�� �����Ƽ� ���ϰ�ħ �񸶸���ġ 0�Ǹ� ���׹̳� ȸ������
        if (player.Sit.HasFlag(PlayerStat.situation.thirst))
        {
            player.Stamina += 0;
        }
    }

}
