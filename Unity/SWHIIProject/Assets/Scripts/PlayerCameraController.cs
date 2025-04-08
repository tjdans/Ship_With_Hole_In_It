using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Follw Settings")]
    [SerializeField] private Transform target; // 따라갈 대상 (플레이어)
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -5); // 카메라 위치 오프셋
    [SerializeField] private float sensitivity = 0.5f; // 마우스 감도
    [SerializeField] private float pitchMin = -30f; // 카메라 상하 회전 최소
    [SerializeField] private float pitchMax = 60f; // 카메라 상하 회전 최대

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 1.0f;
    [SerializeField] private float minZoom = -10.0f; //최대 줌 아웃
    [SerializeField] private float maxZoom = -2.0f; //최대 줌 인

    private float yaw; // 좌우 Y축 회전 값
    private float pitch; // 상하 X축 회전 값

    private Vector2 lookInput; // 인풋 시스템으로 받는 마우스 입력 값
                               // X : 좌우, Y :상하
    private float targetZoomZ;  // 목표 Z 오프셋 값 (카메라 거리)

    private Animator playerAnimator;

    enum playerState
    {
        isIdle,
        isRun,
        isAttack,
        isGliding,
        isDie
    }

    //인풋 시스템에서 마우스 값이 바뀔때 호출되고, 마우스 값을 저장함
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    //인풋 시스템에서 마우스 휠에 의해 호출됨
    public void OnZoom(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<Vector2>().y;

        // 마우스 휠 방향에 따라 줌 거리 조정
        targetZoomZ += scroll * zoomSpeed;
        targetZoomZ = Mathf.Clamp(targetZoomZ, minZoom, maxZoom);
    }


    private void Start()
    {
        playerState state = playerState.isIdle;
        targetZoomZ = offset.z;
    }

    //Update이후에 호출되는 LateUpdate를 쓴 이유는
    //플레이어 움직임(Update에서 처리)를 따라가기 때문
    private void LateUpdate()
    {
        HandleLook();
        FollowTarget();
    }

    //마우스 인풋 값을 기반으로 카메라 회전값을 업데이트하는 함수
    private void HandleLook()
    {
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
    }

    //플레이어(target)의 위치 기준으로 카메라 회전, 오프셋 설정
    private void FollowTarget()
    {
        //대상이 없으면 작동안함
        if (target == null) return;

        // 줌값 반영 서서히 부드럽게 인,아웃이 되도록 설정
        offset.z = Mathf.Lerp(offset.z, targetZoomZ, Time.deltaTime * 10f);
        //마우스 상하, 좌우 값으로 카메라 회전 값을 설정
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        //플레이어의 위치에서 회전된 오프셋 만큼 떨어진 위치에 카메라 배치
        Vector3 targetPosition = target.position + rotation * offset;

        //카메라 위치,회전 적용부분
        transform.position = targetPosition;
        transform.rotation = rotation;
    }

    private void StateUpdate()
    {
        if (target == null) return;
    }
}