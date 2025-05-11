using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEditor.Experimental.GraphView;

public class QuickSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public int slotIndex;

    public void SetItem(Sprite icon)
    {
        iconImage.sprite = icon;
        iconImage.enabled = (icon != null);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Mouse.current.rightButton.wasPressedThisFrame)
        {
         //   QuickSlotManager.Instance.UseItemFromSlot(slotIndex);
        }
    }
}
