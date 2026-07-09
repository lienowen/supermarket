using UnityEngine;

public class CarrySystem : MonoBehaviour
{
    public Transform carryPoint;
    private ProductBox currentBox;

    public bool HasItem()
    {
        return currentBox != null;
    }

    public void Pickup(ProductBox box)
    {
        if(currentBox != null) return;

        currentBox = box;
        box.transform.SetParent(carryPoint);
        box.transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        if(currentBox == null) return;

        currentBox.transform.SetParent(null);
        currentBox = null;
    }
}
