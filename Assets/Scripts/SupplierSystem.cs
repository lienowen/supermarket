using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SupplierData
{
    public string id;
    public string displayName;
    public float priceMultiplier = 1f;
    public float deliverySeconds = 3f;
    public bool unlocked = true;
}

public class SupplierSystem : MonoBehaviour
{
    public static SupplierSystem Instance;
    public List<SupplierData> suppliers = new List<SupplierData>();

    void Awake()
    {
        Instance = this;

        if (suppliers.Count == 0)
        {
            suppliers.Add(new SupplierData(){id="local",displayName="Local Supplier",priceMultiplier=1f,deliverySeconds=2f,unlocked=true});
            suppliers.Add(new SupplierData(){id="wholesale",displayName="Wholesale",priceMultiplier=0.85f,deliverySeconds=6f,unlocked=false});
        }
    }

    public SupplierData GetSupplier(string id)
    {
        return suppliers.Find(x => x.id == id && x.unlocked);
    }
}
