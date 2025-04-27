using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
  
public class QuickSlotManager : MonoBehaviour
{
    //[System.Serializable]
    //public class QuickSlot
    //{
    //    public KeyCode key;
    //    public Image icon;
    //}

    //public QuickSlot[] slots;

    public GameObject[] quickSlotItems; // 슬롯에 있는 아이템들

    //퀵슬롯 1~9까지 함수----------------------
    public void OnQuickSlot1(InputAction.CallbackContext context)
    {
        string quickNumberName = context.control.name;
        int quickIndex = -1;
        switch(quickNumberName)
        {
            case "1": 
                quickIndex = 0;
                break;
            case "2":
                quickIndex = 1;
                break;
            case "3":
                quickIndex = 2;
                break;
            case "4":
                quickIndex = 3;
                break;
            case "5":
                quickIndex = 4;
                break;
            case "6":
                quickIndex = 5;
                break;
            case "7":
                quickIndex = 6;
                break;
            case "8":
                quickIndex = 7;
                break;
            case "9":
                quickIndex = 8;
                break;
            default:
                quickIndex = -1;
                break;
        }
        if(quickIndex != -1)
        {
            UseItemFromSlot(quickIndex);
        }
    }

    private void UseItemFromSlot(int slotIndex)
    {
        if(slotIndex >= 0 && slotIndex < quickSlotItems.Length && quickSlotItems[slotIndex] != null)
        {
            Debug.Log($"QuickSlot {slotIndex + 1} 아이템 사용: {quickSlotItems[slotIndex].name}");
        }
    }
}
