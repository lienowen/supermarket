using UnityEngine;

[CreateAssetMenu(menuName = "Supermarket/Art Runtime Catalog", fileName = "ArtRuntimeCatalog")]
public class ArtRuntimeCatalog : ScriptableObject
{
    [Header("Characters")]
    public Sprite player;
    public Sprite[] customers;

    [Header("Gameplay Objects")]
    public Sprite drinkBox;
    public Sprite shoppingCart;
    public Sprite drinkShelf;
    public Sprite checkoutCounter;

    [Header("Environment")]
    public Sprite warehouseWall;
    public Sprite floor;

    public Sprite GetCustomer(int index)
    {
        if (customers == null || customers.Length == 0)
            return null;

        int safeIndex = Mathf.Abs(index) % customers.Length;
        return customers[safeIndex];
    }
}
