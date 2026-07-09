using UnityEngine;

public class StoreLevelSystem : MonoBehaviour
{
    public int level = 1;
    public int experience = 0;
    public int upgradeNeed = 100;

    public void AddExperience(int value)
    {
        experience += value;

        if(experience >= upgradeNeed)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        experience = 0;
        upgradeNeed += 100;
    }
}
