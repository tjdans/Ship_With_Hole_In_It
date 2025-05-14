using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager Instance { get; private set; }

    [Header("References")]
    public PlayerManager playerManager;
    public Transform itemSpawnPosition; // ������ ������ ���� ��ġ
    private GameObject currentSpawnedItem; // ���� ������ ������ ������

    [Header("Quick Slot Data")]
    public ItemData[] quickSlotItems = new ItemData[9]; // ������ ���� �迭

    [System.Serializable]
    public class QuickSlotUI
    {
        public Image slotIcon;         // ������ ������
        public Image slotFrame;        // �׵θ� �̹���(�����ϸ� ���õ� ������ �׵θ��� ǥ�õǵ���)
    }

    [Header("Quick Slot UI")]
    public QuickSlotUI[] quickSlotUI = new QuickSlotUI[9]; // ���� UI�迭
    public Sprite normalFrameSprite; // �Ϲ� ������ �׵θ�
    public Sprite selectedFrameSprite; // ���õ� ������ �׵θ�

    private int currentSelectedIndex = -1; // ���� ���õ� ������ ���� �ε���

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //UI�ʱ�ȭ
        RefreshAllSlotUI();
    }

    //Input System�� ����Ű �Է� ó��
    public void OnQuickSlot(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        //�Էµ� Ű �̸�(1~9)���� ���ڷ� ��ȯ�ؼ� ����
        if (!int.TryParse(context.control.name, out int keyNum)) return;

        int slotIndex = keyNum - 1;

        // ���� ���� ���� �ִϸ��̼��� ���� ������ ���
        if (playerManager.currentState is MeleeAttack1State || playerManager.currentState is MeleeAttack2State)
        {
            Debug.Log("���� �� - �ִϸ��̼� ���� �� ������ ��ȯ");
            return;  // ���� �ִϸ��̼��� ���� ������ ���
        }

        if (slotIndex >= 0 && slotIndex < quickSlotItems.Length)
        {
            ItemData item = quickSlotItems[slotIndex];
            if (item != null)
            {
                UpdateSlotSelectionUI(slotIndex); // �׵θ� ��ü
                ReplaceItemPrefab(item); // ������ ����,����
                if (playerManager.currentState is IdleState || playerManager.currentState is RunState)
                {
                    Debug.Log(" ture");
                    HandleItemEquip(item);
                }
                else Debug.Log("false");
            }
        }
    }

    //������ ó�� �Լ� 
    public void HandleItemEquip(ItemData item)
    {

        switch (item.itemType)  
        {
            case ItemData.ItemType.MeleeWeapon:
            case ItemData.ItemType.Bow:
                playerManager.EquipWeapon(item); //����,Ȱ ����
                break;

            case ItemData.ItemType.consumable:
                item.Use(playerManager); // �Ҹ�ǰ ���
                break;

            // ���⸦ ����ִ� �ִϸ��̼� ȣ�� �� ó��(��� �����۵鶧 ������ ����ִ� ���⸦ ����ֵ���)
            default:
                playerManager.UnequipWeapon();
                break;
        }
    }

    //���� �׵θ� ������Ʈ
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
        // ���� ������ ����
        if (currentSpawnedItem != null)
        {
            Destroy(currentSpawnedItem);
            currentSpawnedItem = null;
        }

        // �� ������ ����
        if (item.Prefab != null)
        {
            currentSpawnedItem = Instantiate(item.Prefab, itemSpawnPosition.transform);
        }
    }
}