using UnityEngine;

public class PrefabFactory : MonoBehaviour
{
    public GameObject boxPrefab;
    public GameObject cartPrefab;
    public GameObject shelfPrefab;

    public GameObject CreateBox(Vector3 position, string productId)
    {
        GameObject obj = Instantiate(boxPrefab, position, Quaternion.identity);
        ItemBox box = obj.GetComponent<ItemBox>();
        if(box != null)
            box.productId = productId;
        return obj;
    }

    public GameObject CreateCart(Vector3 position)
    {
        return Instantiate(cartPrefab, position, Quaternion.identity);
    }
}
