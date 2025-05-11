using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/EquipableItem")]
public class EquipableItemData : ItemData
{
    public override void Use(PlayerManager player)
    {
        if(itemType ==ItemType.MeleeWeapon || itemType == ItemType.Bow)
        {

        }
    }
}