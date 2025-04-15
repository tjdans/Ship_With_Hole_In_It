using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Follw Settings")]
    [SerializeField] private Transform target; // 따라갈 대상 (플레이어)
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0.0f, 1.6f, -5.0f); // 3인칭 카메라 위치
    [SerializeField] private Vector3 firstPersonOffset = new Vector3(0.0f, 1.6f, 0.0f); // 1인칭 카메라 위치
    [SerializeField] private float sensitivity = 0.2f; // 마우스 감도
    [SerializeField] private float pitchMin = -30f; // 카메라 상하 회전 최소
    [SerializeField] private float pitchMax = 60f; // 카메라 상하 회전 최대
    [SerializeField] private GameObject playerModel;

    [Header("Camera Collision")]
    [SerializeField] private LayerMask collisionMask; // 카메라와 충돌한 레이어
    [SerializeField] private float headOffset = 0.5f; // 카메라 머리 앞쪽 거리
    [SerializeField] private float minCameraDistance = 0.5f; // 충돌한 오브젝트와 카메라의 최소 거리
    [SerializeField] private float collisionRadius = 0.3f;

    private float yaw; // 좌우 Y축 회전 값
    private float pitch; // 상하 X축 회전 값
    private Vector2 lookInput; // 인풋 시스템으로 받는 마우스 입력 값
                               // X : 좌우, Y :상하


    private Vector3 currentOffset; // 현재 카메라 오프셋
    private Vector3 targetOffset; // 목표 카메라 오프셋

    private bool thirdPerson = true; // 현재 시점 상태 (true = 3인칭,false = 1인칭)

    private void Start()
    {
        targetOffset = thirdPersonOffset;
        currentOffset = targetOffset;

    }

    //Update이후에 호출되는 LateUpdate를 쓴 이유는
    //플레이어 움직임(Update에서 처리)를 따라가기 때문
    private void LateUpdate()
    {
        HandleLook();

        if(thirdPerson)
            UpdateThirdPersonCamera();
        else
            UpdateFirstPersonCamera();
    }

    //인풋 시스템에서 마우스 값이 바뀔때 호출되고, 마우스 값을 저장함
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    //인풋 시스템에서 마우스 휠에 의해 호출됨
    // 기존에는 1인칭
    // F5키를 누를 때 1인칭이면 1인칭에서 3인칭으로 변경
    // 만약 3인칭이면 3인칭에서 1인칭으로 변경하는 코드

    public void OnToggleView(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;

        thirdPerson = !thirdPerson;
        targetOffset = thirdPerson ? thirdPersonOffset : firstPersonOffset;
        playerModel.SetActive(thirdPerson);
        Debug.Log("OnToggleView");
    }

    //마우스 인풋 값을 기반으로 카메라 회전값을 업데이트하는 함수
    private void HandleLook()
    {
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
    }

    // 3인칭 카메라 처리
private void UpdateThirdPersonCamera()
{
    if (target == null) return;

    // 회전 적용 후 카메라가 위치할 지점 계산
    Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
    Vector3 desiredCameraPos = target.position + rotation * currentOffset;

    // 벽과의 충돌을 검사하기 위해 시작 위치와 끝 위치 계산
    Vector3 start = target.position + Vector3.up * 1.2f; // 카메라 시작 위치
    Vector3 direction = (desiredCameraPos - start).normalized; // 카메라가 이동할 방향
    float distance = Vector3.Distance(start, desiredCameraPos); // 카메라 이동 거리

    RaycastHit hit;

    // SphereCast로 벽과 충돌을 검사
    if (Physics.SphereCast(start, collisionRadius, direction, out hit, distance, collisionMask))
    {
        // 벽이 있으면 벽과 최소 거리만큼 떨어져서 카메라 위치를 설정
        desiredCameraPos = hit.point - direction * minCameraDistance;
    }

    // 카메라 위치 및 회전 적용
    transform.position = desiredCameraPos;
    transform.rotation = rotation;

    // 카메라가 벽과 겹치지 않도록 밀어내기 처리
    Vector3 cameraPosition = transform.position;
    Vector3 directionToCamera = cameraPosition - target.position;
    float cameraDistance = directionToCamera.magnitude;

    if (cameraDistance < minCameraDistance)
    {
        // 카메라가 너무 가까워지면 밀어내기
        transform.position = target.position + directionToCamera.normalized * minCameraDistance;
    }
}

private void UpdateFirstPersonCamera()
{
    if (target == null) return;

    Vector3 headPos = target.position + Vector3.up * 1.2f; // 플레이어 머리 위치
    Vector3 forward = Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward;

    // 카메라가 이동할 목표 위치
    Vector3 desiredPos = headPos + forward * headOffset;

    // Raycast로 충돌을 확인 (앞, 왼쪽, 오른쪽)
    RaycastHit hit;

    // 앞쪽 Raycast (forward 방향)
    if (Physics.Raycast(headPos, forward, out hit, headOffset + minCameraDistance, collisionMask))
    {
        // 벽이 있다면 카메라가 그 벽 앞쪽에 위치하도록 조정
        desiredPos = hit.point - forward * minCameraDistance;  // 카메라를 벽 앞에 배치
    }

    // 카메라 위치 및 회전 적용
    transform.position = desiredPos;
    transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
}
}