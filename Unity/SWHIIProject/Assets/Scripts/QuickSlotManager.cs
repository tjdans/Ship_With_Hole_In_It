using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager Instance { get; private set; }

    [Header("References")]
    public PlayerManager playerManager;
    public Transform itemSpawnPosition; // 생성될 아이템 스폰 위치
    private GameObject currentSpawnedItem; // 현재 생성된 아이템 프리팹

    [Header("Quick Slot Data")]
    public ItemData[] quickSlotItems = new ItemData[9]; // 아이템 슬롯 배열

    [System.Serializable]
    public class QuickSlotUI
    {
        public Image slotIcon;         // 아이템 아이콘
        public Image slotFrame;        // 테두리 이미지(선택하면 선택된 슬롯의 테두리가 표시되도록)
    }

    [Header("Quick Slot UI")]
    public QuickSlotUI[] quickSlotUI = new QuickSlotUI[9]; // 슬롯 UI배열
    public Sprite normalFrameSprite; // 일반 퀵슬롯 테두리
    public Sprite selectedFrameSprite; // 선택된 상태의 테두리

    private int currentSelectedIndex = -1; // 현재 선택된 퀵슬롯 슬롯 인덱스

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //UI초기화
        RefreshAllSlotUI();
    }

    //Input System의 단축키 입력 처리
    public void OnQuickSlot(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        //입력된 키 이름(1~9)에서 숫자로 변환해서 저장
        if (!int.TryParse(context.control.name, out int keyNum)) return;

        int slotIndex = keyNum - 1;

        // 공격 중일 때는 애니메이션이 끝날 때까지 대기
        if (playerManager.currentState is MeleeAttack1State || playerManager.currentState is MeleeAttack2State)
        {
            Debug.Log("공격 중 - 애니메이션 종료 후 퀵슬롯 전환");
            return;  // 공격 애니메이션이 끝날 때까지 대기
        }

        if (slotIndex >= 0 && slotIndex < quickSlotItems.Length)
        {
            ItemData item = quickSlotItems[slotIndex];
            if (item != null)
            {
                UpdateSlotSelectionUI(slotIndex); // 테두리 교체
                ReplaceItemPrefab(item); // 프리팹 생성,삭제
                if (playerManager.currentState is IdleState || playerManager.currentState is RunState)
                {
                    Debug.Log(" ture");
                    HandleItemEquip(item);
                }
                else Debug.Log("false");
            }
        }
    }

    //아이템 처리 함수 
    public void HandleItemEquip(ItemData item)
    {

        switch (item.itemType)  
        {
            case ItemData.ItemType.MeleeWeapon:
            case ItemData.ItemType.Bow:
                playerManager.EquipWeapon(item); //근접,활 장착
                break;

            case ItemData.ItemType.consumable:
                item.Use(playerManager); // 소모품 사용
                break;

            // 무기를 집어넣는 애니메이션 호출 및 처리(재료 아이템들때 기존에 들고있던 무기를 집어넣도록)
            default:
                playerManager.UnequipWeapon();
                break;
        }
    }

    //슬롯 테두리 업데이트
    private void UpdateSlotSelectionUI(int newIndex)
    {
        if (currentSelectedIndex == newIndex) return;

        if (currentSelectedIndex >= 0 && currentSelectedIndex < quickSlotUI.Length)
        { 
            quickSlotUI[currentSelectedIndex].slotFrame.sprite = normalFrameSprite;
        }

        if (newIndex >= 0 && newIndex < quickSlotUI.Length)
        {
            quickSlotUI[newIndex].slotFrame.sprite = selectedFrameSprite;
        }

        currentSelectedIndex = newIndex;
    }

    public void RefreshAllSlotUI()
    {
        for (int i = 0; i < quickSlotUI.Length; i++)
        {
            if (quickSlotItems[i] != null)
            {
                quickSlotUI[i].slotIcon.sprite = quickSlotItems[i].icon;
                quickSlotUI[i].slotIcon.enabled = true;
            }
            else
            {
                quickSlotUI[i].slotIcon.enabled = false;
            }

            quickSlotUI[i].slotFrame.sprite = (i == currentSelectedIndex) ? selectedFrameSprite : normalFrameSprite;
            Debug.Log($"Slot {i}: item = {quickSlotItems[i]?.name}, icon = {quickSlotItems[i]?.icon}");
        }
    }
   
    private void ReplaceItemPrefab(ItemData item)
    {
        // 이전 프리팹 제거
        if (currentSpawnedItem != null)
        {
            Destroy(currentSpawnedItem);
            currentSpawnedItem = null;
        }

        // 새 프리팹 생성
        if (item.Prefab != null)
        {
            currentSpawnedItem = Instantiate(item.Prefab, itemSpawnPosition.transform);
        }
    }
}