using UnityEngine;

public class CarrySystem : MonoBehaviour
{
    public Transform carryPoint;
    private ProductBox currentBox;

    public bool HasItem()
    {
        return currentBox != null;
    }

    public ProductBox GetCurrentBox()
    {
        return currentBox;
    }

    public bool Pickup(ProductBox box)
    {
        if (box == null || currentBox != null) return false;

        if (carryPoint == null)
            carryPoint = transform;

        currentBox = box;
        box.SetCarried(carryPoint);
        return true;
    }

    public ProductBox Release()
    {
        ProductBox released = currentBox;
        currentBox = null;
        return released;
    }

    public void Drop()
    {
        if (currentBox == null) return;

        ProductBox box = Release();
        box.Drop();
        box.transform.position = transform.position + transform.forward * 1.2f;
    }
}
