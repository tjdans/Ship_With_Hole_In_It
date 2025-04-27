using UnityEngine;

//아이템들의 부모 클래스 
public class Item
{

    public string itemName;
    public Sprite itemImg;

    public virtual void Use()
    {
        Debug.Log("아이템 사용");
    }

}
