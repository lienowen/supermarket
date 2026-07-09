using UnityEngine;

public class PrefabRegistry : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject cartPrefab;
    public GameObject drinkBoxPrefab;
    public GameObject shelfPrefab;
    public GameObject customerPrefab;

    public Transform playerSpawn;
    public Transform cartSpawn;
    public Transform boxSpawn;
    public Transform customerSpawn;

    public void SpawnDay01Objects()
    {
        if(playerPrefab) Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        if(cartPrefab) Instantiate(cartPrefab, cartSpawn.position, Quaternion.identity);
        if(drinkBoxPrefab) Instantiate(drinkBoxPrefab, boxSpawn.position, Quaternion.identity);
        if(customerPrefab) Instantiate(customerPrefab, customerSpawn.position, Quaternion.identity);
    }
}
