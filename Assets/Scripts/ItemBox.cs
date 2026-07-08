using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public string productId = "cola";
    public bool picked;

    public void Pickup()
    {
        picked = true;
        gameObject.SetActive(false);
    }

    public string GetProductId()
    {
        return productId;
    }
}
