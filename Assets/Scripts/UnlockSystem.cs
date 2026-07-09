using UnityEngine;

public class UnlockSystem : MonoBehaviour
{
    public int storeLevel = 1;

    public bool IsUnlocked(string feature)
    {
        switch(feature)
        {
            case "second_shelf":
                return storeLevel >= 2;
            case "second_map":
                return storeLevel >= 3;
            case "premium_products":
                return storeLevel >= 5;
        }

        return true;
    }
}
