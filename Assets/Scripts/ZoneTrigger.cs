using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public enum ZoneType
    {
        Warehouse,
        CartSpawn,
        DrinkAisle,
        Finish
    }

    public ZoneType zoneType;
    public Day01SceneManager sceneManager;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        switch(zoneType)
        {
            case ZoneType.Warehouse:
                sceneManager.EnterWarehouse();
                break;
            case ZoneType.DrinkAisle:
                sceneManager.EnterDrinkAisle();
                break;
            case ZoneType.Finish:
                sceneManager.CompleteDay();
                break;
        }
    }
}
