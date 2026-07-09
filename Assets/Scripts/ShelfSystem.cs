using UnityEngine;

public class ShelfSystem : MonoBehaviour
{
    public string category = "drink";
    public int capacity = 20;
    public int currentCount = 0;

    public bool Restock(ProductBox product)
    {
        if (currentCount >= capacity) return false;
        currentCount++;
        product.gameObject.SetActive(false);
        return true;
    }

    public bool IsComplete()
    {
        return currentCount >= capacity;
    }
}
