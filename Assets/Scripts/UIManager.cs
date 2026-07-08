using UnityEngine;

public class UIManager : MonoBehaviour
{
    public string missionText = "Restock Drinks";
    public int stars = 3;

    public void UpdateMission(string text)
    {
        missionText = text;
    }

    public void SetStars(int value)
    {
        stars = Mathf.Clamp(value,0,3);
    }
}
