using UnityEngine;

public class ShelfSystem : MonoBehaviour, IInteractable
{
    public string category = "drink";
    public int capacity = 20;
    public int currentCount;
    public float cartSearchRadius = 3f;

    public void Interact(GameObject player)
    {
        if (player == null || currentCount >= capacity) return;

        CarrySystem carry = player.GetComponent<CarrySystem>();
        if (carry != null && carry.HasItem())
        {
            ProductBox held = carry.GetCurrentBox();
            if (Restock(held))
                carry.Release();
            return;
        }

        CartSystem cart = FindNearbyCart();
        if (cart == null) return;

        ProductBox fromCart = cart.RemoveOneProduct();
        if (fromCart != null)
            Restock(fromCart);
    }

    public bool Restock(ProductBox product)
    {
        if (product == null || currentCount >= capacity) return false;

        currentCount++;
        product.gameObject.SetActive(false);

        MissionSystem mission = FindObjectOfType<MissionSystem>();
        if (mission != null)
            mission.AddRestock();

        if (StoreLevelSystem.Instance != null)
            StoreLevelSystem.Instance.AddExperience(5);

        return true;
    }

    CartSystem FindNearbyCart()
    {
        CartSystem[] carts = FindObjectsOfType<CartSystem>();
        CartSystem nearest = null;
        float best = cartSearchRadius;

        foreach (CartSystem cart in carts)
        {
            float distance = Vector3.Distance(transform.position, cart.transform.position);
            if (distance <= best)
            {
                best = distance;
                nearest = cart;
            }
        }

        return nearest;
    }

    public bool IsComplete()
    {
        return currentCount >= capacity;
    }
}
