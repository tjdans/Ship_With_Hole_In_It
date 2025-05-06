using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Follw Settings")]
    [SerializeField] private Transform target; // ���� ��� (�÷��̾�)
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0.0f, 1.6f, -5.0f); // 3��Ī ī�޶� ��ġ
    [SerializeField] private Vector3 firstPersonOffset = new Vector3(0.0f, 1.6f, 0.0f); // 1��Ī ī�޶� ��ġ
    [SerializeField] private float sensitivity = 0.2f; // ���콺 ����
    [SerializeField] private float pitchMin = -30f; // ī�޶� ���� ȸ�� �ּ�
    [SerializeField] private float pitchMax = 60f; // ī�޶� ���� ȸ�� �ִ�
    [SerializeField] private GameObject playerModel;

    [Header("Camera Collision")]
    [SerializeField] private LayerMask collisionMask; // ī�޶�� �浹�� ���̾�
    [SerializeField] private float headOffset = 0.5f; // ī�޶� �Ӹ� ���� �Ÿ�
    [SerializeField] private float minCameraDistance = 0.5f; // �浹�� ������Ʈ�� ī�޶��� �ּ� �Ÿ�
    [SerializeField] private float collisionRadius = 0.3f;

    private float yaw; // �¿� Y�� ȸ�� ��
    private float pitch; // ���� X�� ȸ�� ��
    private Vector2 lookInput; // ��ǲ �ý������� �޴� ���콺 �Է� ��
                               // X : �¿�, Y :����


    private Vector3 currentOffset; // ���� ī�޶� ������
    private Vector3 targetOffset; // ��ǥ ī�޶� ������

    private bool thirdPerson = true; // ���� ���� ���� (true = 3��Ī,false = 1��Ī)

    private void Start()
    {
        targetOffset = thirdPersonOffset;
        currentOffset = targetOffset;

    }

    //Update���Ŀ� ȣ��Ǵ� LateUpdate�� �� ������
    //�÷��̾� ������(Update���� ó��)�� ���󰡱� ����
    private void LateUpdate()
    {
        HandleLook();

        if(thirdPerson)
            UpdateThirdPersonCamera();
        else
            UpdateFirstPersonCamera();
    }

    //��ǲ �ý��ۿ��� ���콺 ���� �ٲ� ȣ��ǰ�, ���콺 ���� ������
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    //��ǲ �ý��ۿ��� ���콺 �ٿ� ���� ȣ���
    // �������� 1��Ī
    // F5Ű�� ���� �� 1��Ī�̸� 1��Ī���� 3��Ī���� ����
    // ���� 3��Ī�̸� 3��Ī���� 1��Ī���� �����ϴ� �ڵ�

    public void OnToggleView(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;

        thirdPerson = !thirdPerson;
        targetOffset = thirdPerson ? thirdPersonOffset : firstPersonOffset;
        playerModel.SetActive(thirdPerson);
        Debug.Log("OnToggleView");
    }

    //���콺 ��ǲ ���� ������� ī�޶� ȸ������ ������Ʈ�ϴ� �Լ�
    private void HandleLook()
    {
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
    }

    // 3��Ī ī�޶� ó��
private void UpdateThirdPersonCamera()
{
    if (target == null) return;

    // ȸ�� ���� �� ī�޶� ��ġ�� ���� ���
    Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
    Vector3 desiredCameraPos = target.position + rotation * currentOffset;

    // ������ �浹�� �˻��ϱ� ���� ���� ��ġ�� �� ��ġ ���
    Vector3 start = target.position + Vector3.up * 1.2f; // ī�޶� ���� ��ġ
    Vector3 direction = (desiredCameraPos - start).normalized; // ī�޶� �̵��� ����
    float distance = Vector3.Distance(start, desiredCameraPos); // ī�޶� �̵� �Ÿ�

    RaycastHit hit;

    // SphereCast�� ���� �浹�� �˻�
    if (Physics.SphereCast(start, collisionRadius, direction, out hit, distance, collisionMask))
    {
        // ���� ������ ���� �ּ� �Ÿ���ŭ �������� ī�޶� ��ġ�� ����
        desiredCameraPos = hit.point - direction * minCameraDistance;
    }

    // ī�޶� ��ġ �� ȸ�� ����
    transform.position = desiredCameraPos;
    transform.rotation = rotation;

    // ī�޶� ���� ��ġ�� �ʵ��� �о�� ó��
    Vector3 cameraPosition = transform.position;
    Vector3 directionToCamera = cameraPosition - target.position;
    float cameraDistance = directionToCamera.magnitude;

    if (cameraDistance < minCameraDistance)
    {
        // ī�޶� �ʹ� ��������� �о��
        transform.position = target.position + directionToCamera.normalized * minCameraDistance;
    }
}

private void UpdateFirstPersonCamera()
{
    if (target == null) return;

    Vector3 headPos = target.position + Vector3.up * 1.2f; // �÷��̾� �Ӹ� ��ġ
    Vector3 forward = Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward;

    // ī�޶� �̵��� ��ǥ ��ġ
    Vector3 desiredPos = headPos + forward * headOffset;

    // Raycast�� �浹�� Ȯ�� (��, ����, ������)
    RaycastHit hit;

    // ���� Raycast (forward ����)
    if (Physics.Raycast(headPos, forward, out hit, headOffset + minCameraDistance, collisionMask))
    {
        // ���� �ִٸ� ī�޶� �� �� ���ʿ� ��ġ�ϵ��� ����
        desiredPos = hit.point - forward * minCameraDistance;  // ī�޶� �� �տ� ��ġ
    }

    // ī�޶� ��ġ �� ȸ�� ����
    transform.position = desiredPos;
    transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
}
}