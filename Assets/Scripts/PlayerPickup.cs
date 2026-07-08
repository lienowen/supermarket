using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupDistance = 2f;
    private ItemBox holdingBox;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, pickupDistance))
        {
            ItemBox box = hit.collider.GetComponent<ItemBox>();
            if(box != null)
            {
                holdingBox = box;
                box.Pickup();
                Debug.Log("Picked: " + box.productId);
            }
        }
    }

    public string GetHoldingProduct()
    {
        return holdingBox == null ? "" : holdingBox.productId;
    }

    public void Drop()
    {
        holdingBox = null;
    }
}
