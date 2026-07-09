using UnityEngine;

public class DailyCycleSystem : MonoBehaviour
{
    public int day = 1;
    public float openTime = 0;
    public float closeTime = 300;
    public bool opened;

    public void StartDay()
    {
        opened = true;
        openTime = 0;
    }

    public void EndDay()
    {
        opened = false;
        day++;
        CalculateProfit();
    }

    void CalculateProfit()
    {
        int income = 0;
        if(EconomySystem.Instance != null)
            income = EconomySystem.Instance.totalIncome;

        Debug.Log("Day " + day + " profit: " + income);
    }
}
