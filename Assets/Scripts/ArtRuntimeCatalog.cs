using UnityEngine;

[CreateAssetMenu(menuName = "Supermarket/Art Runtime Catalog", fileName = "ArtRuntimeCatalog")]
public class ArtRuntimeCatalog : ScriptableObject
{
    [Header("Characters")]
    public Sprite player;
    public Sprite playerIdle;
    public Sprite playerCarry;
    public Sprite[] customers;

    [Header("Products")]
    public Sprite drinkBox;
    public Sprite colaBox;
    public Sprite waterBox;
    public Sprite milkBox;
    public Sprite chipsBox;

    [Header("Gameplay Objects")]
    public Sprite shoppingCart;
    public Sprite drinkShelf;
    public Sprite checkoutCounter;
    public Sprite fridgeDoubleDrinks;

    [Header("Day01 Decorations")]
    public Sprite warehouseCorner;
    public Sprite palletBoxStack;
    public Sprite promoStandSuperSale;
    public Sprite pottedPlantLarge;

    [Header("Environment")]
    public Sprite warehouseWall;
    public Sprite wall;
    public Sprite floor;

    [Header("UI")]
    public Sprite missionPanel;
    public Sprite coinIcon;
    public Sprite coinStack;
    public Sprite starIcon;
    public Sprite timerIcon;
    public Sprite buttonPlay;
    public Sprite buttonUpgrade;
    public Sprite buttonNext;

    public Sprite GetCustomer(int index)
    {
        if (customers == null || customers.Length == 0)
            return null;

        int safeIndex = Mathf.Abs(index) % customers.Length;
        return customers[safeIndex];
    }

    public Sprite GetProduct(string productId)
    {
        switch (productId)
        {
            case "cola_box":
                return colaBox != null ? colaBox : drinkBox;
            case "water_box":
                return waterBox;
            case "milk_box":
                return milkBox;
            case "chips_box":
                return chipsBox;
            default:
                return drinkBox;
        }
    }
}
