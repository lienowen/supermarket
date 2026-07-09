using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int currentMap = 1;

    public void LoadMap(int id)
    {
        currentMap = id;
        Debug.Log("Load Map " + id);
    }

    public bool CanEnterMap(int id)
    {
        return id <= currentMap + 1;
    }
}
