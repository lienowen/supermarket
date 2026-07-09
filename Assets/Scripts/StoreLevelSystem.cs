using UnityEngine;

public class StoreLevelSystem : MonoBehaviour
{
    public static StoreLevelSystem Instance;

    public int level = 1;
    public int experience;
    public int upgradeNeed = 100;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddExperience(int value)
    {
        if (value <= 0) return;

        experience += value;

        while (experience >= upgradeNeed)
        {
            experience -= upgradeNeed;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        upgradeNeed = Mathf.RoundToInt(upgradeNeed * 1.35f);
    }
}
