using UnityEngine;
using System.Collections.Generic;

public class CartSystem : MonoBehaviour
{
    public int capacity = 10;
    private List<ProductBox> items = new List<ProductBox>();

    public bool AddProduct(ProductBox item)
    {
        if (items.Count >= capacity) return false;
        items.Add(item);
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        return true;
    }

    public int GetCount()
    {
        return items.Count;
    }

    public void Clear()
    {
        items.Clear();
    }
}
