#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class Day01SceneRepair
{
    private const string ScenePath = "Assets/Scenes/Day01.unity";
    private const string SessionKey = "supermarket.day01.scene.repaired";

    static Day01SceneRepair()
    {
        EditorApplication.delayCall += EnsureValidScene;
    }

    [MenuItem("Supermarket/Rebuild Day01 Scene")]
    public static void RebuildScene()
    {
        BuildFreshScene();
    }

    private static void EnsureValidScene()
    {
        if (SessionState.GetBool(SessionKey, false)) return;
        SessionState.SetBool(SessionKey, true);

        FileInfo file = new FileInfo(ScenePath);
        if (!file.Exists || file.Length < 1024)
            BuildFreshScene();
    }

    private static void BuildFreshScene()
    {
        Directory.CreateDirectory("Assets/Scenes");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject root = new GameObject("Day01");
        GameObject lightObject = new GameObject("Directional Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.1f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        cameraObject.transform.position = new Vector3(0f, 8f, -8f);
        cameraObject.transform.rotation = Quaternion.Euler(35f, 0f, 0f);

        root.transform.position = Vector3.zero;

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.Refresh();

        Debug.Log("Day01SceneRepair: rebuilt a valid Day01 scene. Runtime content will be installed automatically.");
    }
}
#endif
