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
