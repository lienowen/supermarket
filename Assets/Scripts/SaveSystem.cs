using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public void Save(int day, int coin, int level)
    {
        PlayerPrefs.SetInt("Day", day);
        PlayerPrefs.SetInt("Coin", coin);
        PlayerPrefs.SetInt("StoreLevel", level);
        PlayerPrefs.Save();
    }

    public void SaveEconomy()
    {
        if(EconomySystem.Instance == null) return;

        PlayerPrefs.SetInt("Coin", EconomySystem.Instance.money);
        PlayerPrefs.SetInt("Income", EconomySystem.Instance.totalIncome);
        PlayerPrefs.Save();
    }

    public void LoadEconomy()
    {
        if(EconomySystem.Instance == null) return;

        EconomySystem.Instance.money = PlayerPrefs.GetInt("Coin", 1000);
        EconomySystem.Instance.totalIncome = PlayerPrefs.GetInt("Income", 0);
    }

    public int GetDay()
    {
        return PlayerPrefs.GetInt("Day", 1);
    }

    public int GetCoin()
    {
        return PlayerPrefs.GetInt("Coin", 500);
    }

    public int GetStoreLevel()
    {
        return PlayerPrefs.GetInt("StoreLevel", 1);
    }
}
