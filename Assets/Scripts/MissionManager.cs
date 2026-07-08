using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public int target = 10;
    public int current = 0;

    public void AddRestock()
    {
        current++;
        if(current >= target)
        {
            Debug.Log("MISSION COMPLETE");
        }
    }
}
