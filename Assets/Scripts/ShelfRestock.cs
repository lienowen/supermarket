using UnityEngine;

public class ShelfRestock : MonoBehaviour
{
    public string productId = "cola";
    public int stock = 0;
    public int maxStock = 50;

    public bool Restock(string product)
    {
        if(product != productId)
            return false;

        if(stock >= maxStock)
            return false;

        stock++;

        if(GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(10);
        }

        return true;
    }
}
