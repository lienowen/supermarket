using UnityEngine;

public class ShopHUDSystem : MonoBehaviour
{
    public int money;
    public int missionProgress;
    public int storeLevel;

    void Update()
    {
        if(EconomySystem.Instance != null)
            money = EconomySystem.Instance.money;

        if(StoreLevelSystem.Instance != null)
            storeLevel = StoreLevelSystem.Instance.level;
    }

    public void UpdateMission(int value)
    {
        missionProgress = value;
    }
}
