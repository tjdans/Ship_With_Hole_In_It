using UnityEngine;

//�����۵��� �θ� Ŭ���� 
public class Item
{

    public string itemName;
    public Sprite itemImg;

    public virtual void Use()
    {
        Debug.Log("������ ���");
    }

}
