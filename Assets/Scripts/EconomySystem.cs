using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    public static EconomySystem Instance;

    [Header("Wallet")]
    public int money = 500;

    [Header("Day Totals")]
    public int totalIncome;
    public int totalExpense;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool Spend(int amount)
    {
        if (amount < 0 || money < amount)
            return false;

        money -= amount;
        totalExpense += amount;
        return true;
    }

    public bool BuyProduct(int price)
    {
        return Spend(price);
    }

    public void AddIncome(int amount)
    {
        if (amount <= 0) return;

        money += amount;
        totalIncome += amount;
    }

    public void SellProduct(int price)
    {
        AddIncome(price);
    }

    public void AddReward(int amount)
    {
        AddIncome(amount);
    }

    public int GetProfit()
    {
        return totalIncome - totalExpense;
    }

    public void ResetDayTotals()
    {
        totalIncome = 0;
        totalExpense = 0;
    }
}
