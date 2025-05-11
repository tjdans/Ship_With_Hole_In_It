using UnityEngine;

//아이템들의 부모 클래스 
[CreateAssetMenu(fileName = "NewItem", menuName = "Item/GenericItem")]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        None,
        Material,//재료
        consumable,//소모품
        MeleeWeapon,//근접무기
        Bow//활
    }

    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public GameObject Prefab;

    public virtual void Use(PlayerManager player)
    {
        Debug.Log("현재 들고 있는 아이템 타입" );
    }

}
