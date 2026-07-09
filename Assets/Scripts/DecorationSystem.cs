using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DecorationItem
{
    public string id;
    public int price;
    public bool placed;
}

public class DecorationSystem : MonoBehaviour
{
    public List<DecorationItem> items = new List<DecorationItem>();

    public bool BuyDecoration(string id,int money)
    {
        if(money < 0) return false;

        items.Add(new DecorationItem(){id=id,price=money,placed=false});
        return true;
    }

    public void Place(string id)
    {
        DecorationItem item = items.Find(x=>x.id==id);
        if(item != null)
            item.placed = true;
    }
}
