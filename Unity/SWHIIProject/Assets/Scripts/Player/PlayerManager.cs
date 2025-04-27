using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    //-------------조작 외에 플레이어한테 필요한 것들-------------
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    //--------------조작에 관련 부분 --------------------------
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float rollSpeed = 8.0f; // 구르는 스피드인데 거리에도 영향을 줌
    [SerializeField] private float rollDuration = 1.18f;

    private CharacterController controller;
    private Animator animator;
    private PlayerInput playerInput;

    private Vector2 moveInput; // 움직이는 키 입력 받으면 이동 방향을 저장할 변수
    private Vector2 rollDirection; //구르는 방향 Speed에 영향 받음
    private Vector3 velocity; // 중력값 변수

    private float rollTimer; // 구르는 시간 타이머 변수 

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

    //중력처리 담당 함수
    private void ApplyGravity()
    {
        
    }
}
