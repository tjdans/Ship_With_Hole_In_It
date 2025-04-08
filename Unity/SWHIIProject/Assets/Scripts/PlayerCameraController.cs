using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Follw Settings")]
    [SerializeField] private Transform target; // ���� ��� (�÷��̾�)
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -5); // ī�޶� ��ġ ������
    [SerializeField] private float sensitivity = 0.5f; // ���콺 ����
    [SerializeField] private float pitchMin = -30f; // ī�޶� ���� ȸ�� �ּ�
    [SerializeField] private float pitchMax = 60f; // ī�޶� ���� ȸ�� �ִ�

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 1.0f;
    [SerializeField] private float minZoom = -10.0f; //�ִ� �� �ƿ�
    [SerializeField] private float maxZoom = -2.0f; //�ִ� �� ��

    private float yaw; // �¿� Y�� ȸ�� ��
    private float pitch; // ���� X�� ȸ�� ��

    private Vector2 lookInput; // ��ǲ �ý������� �޴� ���콺 �Է� ��
                               // X : �¿�, Y :����
    private float targetZoomZ;  // ��ǥ Z ������ �� (ī�޶� �Ÿ�)

    private Animator playerAnimator;

    enum playerState
    {
        isIdle,
        isRun,
        isAttack,
        isGliding,
        isDie
    }

    //��ǲ �ý��ۿ��� ���콺 ���� �ٲ� ȣ��ǰ�, ���콺 ���� ������
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    //��ǲ �ý��ۿ��� ���콺 �ٿ� ���� ȣ���
    public void OnZoom(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<Vector2>().y;

        // ���콺 �� ���⿡ ���� �� �Ÿ� ����
        targetZoomZ += scroll * zoomSpeed;
        targetZoomZ = Mathf.Clamp(targetZoomZ, minZoom, maxZoom);
    }


    private void Start()
    {
        playerState state = playerState.isIdle;
        targetZoomZ = offset.z;
    }

    //Update���Ŀ� ȣ��Ǵ� LateUpdate�� �� ������
    //�÷��̾� ������(Update���� ó��)�� ���󰡱� ����
    private void LateUpdate()
    {
        HandleLook();
        FollowTarget();
    }

    //���콺 ��ǲ ���� ������� ī�޶� ȸ������ ������Ʈ�ϴ� �Լ�
    private void HandleLook()
    {
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
    }

    //�÷��̾�(target)�� ��ġ �������� ī�޶� ȸ��, ������ ����
    private void FollowTarget()
    {
        //����� ������ �۵�����
        if (target == null) return;

        // �ܰ� �ݿ� ������ �ε巴�� ��,�ƿ��� �ǵ��� ����
        offset.z = Mathf.Lerp(offset.z, targetZoomZ, Time.deltaTime * 10f);
        //���콺 ����, �¿� ������ ī�޶� ȸ�� ���� ����
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        //�÷��̾��� ��ġ���� ȸ���� ������ ��ŭ ������ ��ġ�� ī�޶� ��ġ
        Vector3 targetPosition = target.position + rotation * offset;

        //ī�޶� ��ġ,ȸ�� ����κ�
        transform.position = targetPosition;
        transform.rotation = rotation;
    }

    private void StateUpdate()
    {
        if (target == null) return;
    }
}