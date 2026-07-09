using UnityEngine;
using System.Collections;

public class PurchaseSystem : MonoBehaviour
{
    public InventorySystem inventory;

    public bool Order(string productId, int count, string supplierId = "local")
    {
        if (count <= 0 || inventory == null) return false;
        if (ProductDatabase.Instance == null || SupplierSystem.Instance == null || EconomySystem.Instance == null) return false;

        ProductData product = ProductDatabase.Instance.GetProduct(productId);
        SupplierData supplier = SupplierSystem.Instance.GetSupplier(supplierId);
        if (product == null || supplier == null) return false;

        int totalCost = Mathf.CeilToInt(product.buyPrice * supplier.priceMultiplier * count);
        if (!EconomySystem.Instance.Spend(totalCost)) return false;

        StartCoroutine(Deliver(productId, count, supplier.deliverySeconds));
        return true;
    }

    IEnumerator Deliver(string productId, int count, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        inventory.Add(productId, count);
    }
}
