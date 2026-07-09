using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryItem
{
    public string productId;
    public int amount;
}

public class InventorySystem : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>();

    public void Add(string id, int count)
    {
        InventoryItem item = items.Find(x=>x.productId==id);

        if(item == null)
        {
            item = new InventoryItem();
            item.productId = id;
            items.Add(item);
        }

        item.amount += count;
    }

    public bool Remove(string id, int count)
    {
        InventoryItem item = items.Find(x=>x.productId==id);

        if(item == null || item.amount < count)
            return false;

        item.amount -= count;
        return true;
    }
}
