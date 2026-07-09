using UnityEngine;
using System.Collections.Generic;

public class CartSystem : MonoBehaviour, IInteractable
{
    public int capacity = 10;
    public float followDistance = 1.8f;
    public float followSpeed = 8f;

    private readonly List<ProductBox> items = new List<ProductBox>();
    private Transform pusher;

    void Update()
    {
        if (pusher == null) return;

        Vector3 target = pusher.position + pusher.forward * followDistance;
        target.y = transform.position.y;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followSpeed);
        transform.forward = pusher.forward;
    }

    public void Interact(GameObject player)
    {
        if (player == null) return;

        CarrySystem carry = player.GetComponent<CarrySystem>();
        if (carry != null && carry.HasItem())
        {
            ProductBox box = carry.GetCurrentBox();
            if (AddProduct(box))
                carry.Release();
            return;
        }

        pusher = pusher == player.transform ? null : player.transform;
    }

    public bool AddProduct(ProductBox item)
    {
        if (item == null || items.Count >= capacity) return false;

        items.Add(item);
        item.picked = false;
        item.currentHolder = null;
        item.transform.SetParent(transform);
        item.transform.localPosition = new Vector3(0, 0.8f + items.Count * 0.08f, 0);
        item.transform.localRotation = Quaternion.identity;
        return true;
    }

    public ProductBox RemoveOneProduct()
    {
        if (items.Count == 0) return null;

        int index = items.Count - 1;
        ProductBox item = items[index];
        items.RemoveAt(index);
        item.transform.SetParent(null);
        return item;
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
