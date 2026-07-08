using UnityEngine;

public class ShoppingCart : MonoBehaviour
{
    public int capacity = 10;
    public int currentLoad = 0;

    public bool AddProduct()
    {
        if(currentLoad >= capacity)
            return false;

        currentLoad++;
        return true;
    }

    public void RemoveProduct()
    {
        if(currentLoad > 0)
            currentLoad--;
    }
}
