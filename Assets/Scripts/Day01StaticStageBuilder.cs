using UnityEngine;

/// <summary>
/// Day01 logical coordinates only. Visual composition is handled by
/// Day01ScreenPresentation so sprite scale/orientation cannot break the scene.
/// </summary>
public static class Day01StaticStageBuilder
{
    public static Day01EnvironmentLayout Build()
    {
        CleanupOldRuntimeWorld();

        return new Day01EnvironmentLayout
        {
            playerSpawn = new Vector3(-7.4f, 0f, 0.4f),
            boxOrigin = new Vector3(-8.2f, 0.05f, -3.8f),
            cartPosition = new Vector3(-4.1f, 0.05f, -1.2f),
            shelfPosition = new Vector3(4.1f, 0.05f, -1.5f),
            checkoutPosition = new Vector3(6.3f, 0.05f, 4.35f),
            customerSpawn = new Vector3(-8.9f, 0f, 6.7f),
            customerShelfPoint = new Vector3(2.85f, 0f, -1.45f),
            customerCheckoutPoint = new Vector3(5.0f, 0f, 4.35f),
            customerExitPoint = new Vector3(-8.9f, 0f, 6.7f)
        };
    }

    static void CleanupOldRuntimeWorld()
    {
        DestroyNamed("Environment");
        DestroyNamed("DesignedDay01Decorations");
        DestroyNamed("Day01StaticStage");

        PlayerController oldPlayer = Object.FindObjectOfType<PlayerController>();
        if (oldPlayer != null)
            Object.Destroy(oldPlayer.gameObject);
    }

    static void DestroyNamed(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
            Object.Destroy(obj);
    }
}