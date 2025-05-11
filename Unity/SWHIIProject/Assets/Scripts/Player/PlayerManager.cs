using System;
using System.Net;
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

    public int comboStep = 0;

    private PlayerStateMachine stateMachine;
    public PlayerState currentState => stateMachine.currentState;
    public PlayerStat player;
    private ItemData equippedWeapon;
    public Action OnComboInput;

    private void Awake()
    {
        player = GetComponent<PlayerStat>();
        player = new PlayerStat();
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
        CharacterStat();
    }

    //무기를 드는 함수(무기가 있는 퀵슬롯을 선택 했을 때)
    public void EquipWeapon(ItemData weapon)
    {
        if (equippedWeapon == weapon) return;

        equippedWeapon = weapon;
       // animator.SetTrigger("DrawWeapon");
        stateMachine.ChageState(new MeleeIdleState(this, stateMachine));
    }

    //무기 해제하는 함수(무기가 있는 퀵슬롯에서 다른 퀵슬롯으로 선택되었을 때 )
    public void UnequipWeapon()
    {
        animator.SetBool("WeaponIdle", false);
        if (equippedWeapon == null) return;

        equippedWeapon = null;
        stateMachine.ChageState(new IdleState(this, stateMachine));

    }

    //무기를 들었는지 확인용
    public bool IsWeaponEquipped() => equippedWeapon != null;

    public bool CurrentStateIs(Type stateType)
    {
        return currentState != null && currentState.GetType() == stateType;
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
            player.ThirstyStat -= player.Staminaregeneration * (int)Time.time;
            moveSpeed = 5.0f;
        }
        //변수명 고치기 귀찮아서 내일고침 목마름수치 0되면 스테미너 회복정지
        //플레이어 목마름수치가 50%이하일 경우 스테미너 회복 속도 감소
        if (player.Sit.HasFlag(PlayerStat.situation.smallthirst))
        {
            player.Stamina += player.Staminaregeneration * (int)Time.time / 2;
        }
        if (player.Sit.HasFlag(PlayerStat.situation.thirst))
        {
            player.Stamina += player.Staminaregeneration * 0;
        }
        /*
        if (!player.Sit.HasFlag(PlayerStat.situation.thirst)&&!player.Sit.HasFlag(PlayerStat.situation.smallthirst)&&currentState == PlayerState.idle)
        {
            player.Stamina += player.Staminaregeneration * (int)Time.time;
        }
        */
        if (isGliding == true)
        {
            player.GlidingStat -= Time.deltaTime * 2f;
            Debug.Log(player.GlidingStat);
            if (player.GlidingStat <= 0)
            {
                //  StopGlideAndJumpAgain();
                // 현식 코드 바뀌어서 다 클래스화로 바뀌어서 일단 떨어지는 애니메이션으로 바뀌게 수정할게
                stateMachine.ChageState(new JumpState(this, stateMachine));
                animator.SetTrigger("JumpFromIdle");
            }
        }
        else
        {
            player.GlidingStat += Time.deltaTime;
            Debug.Log(player.GlidingStat);
        }
    }
   }
