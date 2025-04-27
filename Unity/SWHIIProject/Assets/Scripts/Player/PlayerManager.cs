using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    //-------------���� �ܿ� �÷��̾����� �ʿ��� �͵�-------------
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    //--------------���ۿ� ���� �κ� --------------------------
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float rollSpeed = 8.0f; // ������ ���ǵ��ε� �Ÿ����� ������ ��
    [SerializeField] private float rollDuration = 1.18f;

    private CharacterController controller;
    private Animator animator;
    private PlayerInput playerInput;

    private Vector2 moveInput; // �����̴� Ű �Է� ������ �̵� ������ ������ ����
    private Vector2 rollDirection; //������ ���� Speed�� ���� ����
    private Vector3 velocity; // �߷°� ����

    private float rollTimer; // ������ �ð� Ÿ�̸� ���� 

    private bool isJumpPressed;
    private bool isRolling;
    private bool isGliding;

    private enum PlayerState { Idel, Run, Jump, Gliding, Roll}
    private PlayerState currentState = PlayerState.Idel;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        ApplyGravity();

    }

    //�߷�ó�� ��� �Լ�
    private void ApplyGravity()
    {
        
    }
}
