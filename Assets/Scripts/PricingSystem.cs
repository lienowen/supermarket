using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PriceOverride
{
    public string productId;
    public int sellPrice;
}

public class PricingSystem : MonoBehaviour
{
    public List<PriceOverride> overrides = new List<PriceOverride>();

    public int GetSellPrice(string productId)
    {
        PriceOverride custom = overrides.Find(x => x.productId == productId);
        if (custom != null)
            return Mathf.Max(1, custom.sellPrice);

        if (ProductDatabase.Instance == null)
            return 1;

        ProductData data = ProductDatabase.Instance.GetProduct(productId);
        return data != null ? Mathf.Max(1, data.sellPrice) : 1;
    }

    public void SetSellPrice(string productId, int price)
    {
        PriceOverride custom = overrides.Find(x => x.productId == productId);
        if (custom == null)
        {
            custom = new PriceOverride(){productId = productId};
            overrides.Add(custom);
        }

        custom.sellPrice = Mathf.Max(1, price);
    }
}
