using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public int storeLevel = 1;
    public int upgradeCost = 500;

    public bool Upgrade(EconomySystem economy)
    {
        if(economy.money < upgradeCost)
            return false;

        economy.money -= upgradeCost;
        storeLevel++;
        upgradeCost *= 2;

        return true;
    }
}
