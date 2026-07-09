using UnityEngine;

public class Day01SceneManager : MonoBehaviour
{
    public Transform warehouseSpawn;
    public Transform cartSpawn;
    public Transform drinkAisle;
    public Transform finishArea;

    public enum Area
    {
        Warehouse,
        CartArea,
        DrinkAisle,
        Finish
    }

    public Area currentArea = Area.Warehouse;

    public void EnterWarehouse()
    {
        currentArea = Area.Warehouse;
    }

    public void EnterDrinkAisle()
    {
        currentArea = Area.DrinkAisle;
    }

    public void CompleteDay()
    {
        currentArea = Area.Finish;
        ScoreSystem.Instance.CompleteMission();
    }
}
