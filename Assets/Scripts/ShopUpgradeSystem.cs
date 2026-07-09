using UnityEngine;

public class ShopUpgradeSystem : MonoBehaviour
{
    public int shelfLevel = 1;
    public int storeLevel = 1;
    public int maxCustomer = 10;

    public bool UpgradeShelf(int cost)
    {
        if(EconomySystem.Instance == null) return false;
        if(EconomySystem.Instance.money < cost) return false;

        EconomySystem.Instance.money -= cost;
        shelfLevel++;
        return true;
    }

    public void UpgradeStore()
    {
        storeLevel++;
        maxCustomer += 5;
    }
}
