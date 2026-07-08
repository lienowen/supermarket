using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ProductData
{
    public string id;
    public string name;
    public int buyPrice;
    public int sellPrice;
}

public class ProductDatabase : MonoBehaviour
{
    public List<ProductData> products = new List<ProductData>();

    void Awake()
    {
        products.Add(new ProductData(){id="cola",name="Cola",buyPrice=2,sellPrice=5});
        products.Add(new ProductData(){id="water",name="Water",buyPrice=1,sellPrice=3});
        products.Add(new ProductData(){id="milk",name="Milk",buyPrice=3,sellPrice=7});
        products.Add(new ProductData(){id="chips",name="Chips",buyPrice=2,sellPrice=6});
    }

    public ProductData GetProduct(string id)
    {
        return products.Find(x=>x.id==id);
    }
}
