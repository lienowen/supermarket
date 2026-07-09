using UnityEngine;

public class CustomerShoppingSystem : MonoBehaviour
{
    public string targetProduct = "cola_box";
    public bool bought;

    public bool BuyProduct()
    {
        if (bought) return true;

        ProductData product = ProductDatabase.Instance != null
            ? ProductDatabase.Instance.GetProduct(targetProduct)
            : null;

        ShelfSystem[] shelves = FindObjectsOfType<ShelfSystem>();
        foreach (ShelfSystem shelf in shelves)
        {
            if (shelf == null || shelf.currentCount <= 0) continue;
            if (product != null && shelf.category != product.category) continue;

            shelf.currentCount--;
            bought = true;
            return true;
        }

        return false;
    }
}
