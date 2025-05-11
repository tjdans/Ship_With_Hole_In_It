using UnityEngine;

//�����۵��� �θ� Ŭ���� 
[CreateAssetMenu(fileName = "NewItem", menuName = "Item/GenericItem")]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        None,
        Material,//���
        consumable,//�Ҹ�ǰ
        MeleeWeapon,//��������
        Bow//Ȱ
    }

    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public GameObject Prefab;

    public virtual void Use(PlayerManager player)
    {
        Debug.Log("���� ��� �ִ� ������ Ÿ��" );
    }

}
