using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ProductData
{
    public string id;
    public string name;
    public string category;
    public int buyPrice;
    public int sellPrice;
}

public class ProductDatabase : MonoBehaviour
{
    public static ProductDatabase Instance;
    public List<ProductData> products = new List<ProductData>();

    void Awake()
    {
        Instance = this;

        if (products.Count == 0)
            LoadDefaults();
    }

    void LoadDefaults()
    {
        products.Add(new ProductData(){id="cola_box",name="Cola",category="drink",buyPrice=20,sellPrice=35});
        products.Add(new ProductData(){id="water_box",name="Water",category="drink",buyPrice=15,sellPrice=28});
        products.Add(new ProductData(){id="milk_box",name="Milk",category="drink",buyPrice=25,sellPrice=42});
        products.Add(new ProductData(){id="chips_box",name="Chips",category="snack",buyPrice=18,sellPrice=32});
    }

    public ProductData GetProduct(string id)
    {
        return products.Find(x => x.id == id);
    }
}
