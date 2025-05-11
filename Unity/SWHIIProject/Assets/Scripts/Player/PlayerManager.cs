using System;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    //���� ������ �ڵ�
    //------------- ���� �ܿ� �÷��̾����� �ʿ��� �͵� -------------
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

    //���⸦ ��� �Լ�(���Ⱑ �ִ� �������� ���� ���� ��)
    public void EquipWeapon(ItemData weapon)
    {
        if (equippedWeapon == weapon) return;

        equippedWeapon = weapon;
       // animator.SetTrigger("DrawWeapon");
        stateMachine.ChageState(new MeleeIdleState(this, stateMachine));
    }

    //���� �����ϴ� �Լ�(���Ⱑ �ִ� �����Կ��� �ٸ� ���������� ���õǾ��� �� )
    public void UnequipWeapon()
    {
        animator.SetBool("WeaponIdle", false);
        if (equippedWeapon == null) return;

        equippedWeapon = null;
        stateMachine.ChageState(new IdleState(this, stateMachine));

    }

    //���⸦ ������� Ȯ�ο�
    public bool IsWeaponEquipped() => equippedWeapon != null;

    public bool CurrentStateIs(Type stateType)
    {
        return currentState != null && currentState.GetType() == stateType;
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
            player.ThirstyStat -= player.Staminaregeneration * (int)Time.time;
            moveSpeed = 5.0f;
        }
        //������ ��ġ�� �����Ƽ� ���ϰ�ħ �񸶸���ġ 0�Ǹ� ���׹̳� ȸ������
        //�÷��̾� �񸶸���ġ�� 50%������ ��� ���׹̳� ȸ�� �ӵ� ����
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
                // ���� �ڵ� �ٲ� �� Ŭ����ȭ�� �ٲ� �ϴ� �������� �ִϸ��̼����� �ٲ�� �����Ұ�
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
