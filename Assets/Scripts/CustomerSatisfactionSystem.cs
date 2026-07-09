using UnityEngine;

public class CustomerSatisfactionSystem : MonoBehaviour
{
    [Range(0, 100)] public float satisfaction = 75f;
    public int happyCustomers;
    public int unhappyCustomers;

    public void RecordCheckout(float waitSeconds, bool foundProduct)
    {
        float delta = 0f;

        if (foundProduct) delta += 6f;
        else delta -= 15f;

        if (waitSeconds < 10f) delta += 5f;
        else if (waitSeconds > 30f) delta -= 10f;

        satisfaction = Mathf.Clamp(satisfaction + delta, 0f, 100f);

        if (delta >= 0f) happyCustomers++;
        else unhappyCustomers++;
    }

    public float GetCustomerSpawnMultiplier()
    {
        return Mathf.Lerp(0.6f, 1.5f, satisfaction / 100f);
    }
}
