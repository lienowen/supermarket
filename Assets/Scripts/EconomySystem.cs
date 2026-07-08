using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    public int money = 500;

    public bool BuyProduct(int price)
    {
        if(money < price)
            return false;

        money -= price;
        return true;
    }

    public void SellProduct(int price)
    {
        money += price;
    }

    public void AddReward(int amount)
    {
        money += amount;
    }
}
