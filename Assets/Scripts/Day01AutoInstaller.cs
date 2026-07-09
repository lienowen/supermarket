using UnityEngine;
using UnityEngine.SceneManagement;

public static class Day01AutoInstaller
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "Day01") return;
        if (Object.FindObjectOfType<Day01TapFlowController>() != null) return;

        BuildPlayableDay01();
    }

    static void BuildPlayableDay01()
    {
        GameObject systems = new GameObject("GameSystems");

        systems.AddComponent<EconomySystem>();
        systems.AddComponent<StoreLevelSystem>();
        systems.AddComponent<ScoreSystem>();

        MissionSystem mission = systems.AddComponent<MissionSystem>();
        mission.targetAmount = 6;
        mission.reward = 200;

        GameStateManager state = systems.AddComponent<GameStateManager>();
        systems.AddComponent<ProductDatabase>();
        systems.AddComponent<SupplierSystem>();
        systems.AddComponent<PricingSystem>();
        systems.AddComponent<CustomerSatisfactionSystem>();
        systems.AddComponent<AchievementSystem>();

        InventorySystem inventory = systems.AddComponent<InventorySystem>();
        PurchaseSystem purchase = systems.AddComponent<PurchaseSystem>();
        purchase.inventory = inventory;

        DailyCycleSystem day = systems.AddComponent<DailyCycleSystem>();
        day.StartDay();
        state.StartGame();

        Camera gameplayCamera = ConfigureFixedCamera();
        Day01EnvironmentLayout layout = Day01StaticStageBuilder.Build();

        // Logical objects only. Day01ScreenPresentation draws the visible scene.
        for (int i = 0; i < 6; i++)
        {
            int column = i % 3;
            int row = i / 3;
            Vector3 position = layout.boxOrigin + new Vector3(column * 1.05f, 0f, row * 0.92f);

            GameObject box = CreateColliderRoot(
                "DrinkBox_" + (i + 1),
                position,
                new Vector3(0.92f, 0.78f, 0.78f),
                new Vector3(0f, 0.39f, 0f)
            );

            ProductBox product = box.AddComponent<ProductBox>();
            product.productId = "cola_box";
        }

        GameObject cartObject = CreateColliderRoot(
            "ShoppingCart",
            layout.cartPosition,
            new Vector3(1.55f, 2.05f, 1.35f),
            new Vector3(0f, 1f, 0f)
        );
        CartSystem cart = cartObject.AddComponent<CartSystem>();
        cart.capacity = 6;

        GameObject shelfObject = CreateColliderRoot(
            "DrinkShelf",
            layout.shelfPosition,
            new Vector3(2.55f, 3.45f, 1.25f),
            new Vector3(0f, 1.72f, 0f)
        );
        ShelfSystem shelf = shelfObject.AddComponent<ShelfSystem>();
        shelf.capacity = 6;
        shelf.category = "drink";

        Transform customerSpawn = CreatePoint("CustomerSpawn", layout.customerSpawn);
        Transform shelfPoint = CreatePoint("CustomerShelfPoint", layout.customerShelfPoint);
        Transform checkoutPoint = CreatePoint("CustomerCheckoutPoint", layout.customerCheckoutPoint);
        Transform exitPoint = CreatePoint("CustomerExitPoint", layout.customerExitPoint);

        GameObject checkout = CreateColliderRoot(
            "CheckoutCounter",
            layout.checkoutPosition,
            new Vector3(2.8f, 3.1f, 1.4f),
            new Vector3(0f, 1.5f, 0f)
        );

        CheckoutQueueSystem checkoutQueue = checkout.AddComponent<CheckoutQueueSystem>();
        checkoutQueue.checkoutPoint = checkoutPoint;
        checkoutQueue.queueDirection = Vector3.left;
        checkoutQueue.spacing = 1.1f;
        checkoutQueue.maxQueue = 6;
        checkoutQueue.serviceSeconds = 2f;
        checkoutQueue.autoServe = true;
        checkoutQueue.cashierAvailable = true;

        CheckoutSystem checkoutSystem = checkout.AddComponent<CheckoutSystem>();
        checkoutSystem.queueSystem = checkoutQueue;
        checkoutSystem.queueLimit = 6;

        CustomerSpawner spawner = systems.AddComponent<CustomerSpawner>();
        spawner.autoSpawn = false;
        spawner.createWorldVisuals = false;
        spawner.spawnPoint = customerSpawn;
        spawner.shelfPoint = shelfPoint;
        spawner.checkoutQueue = checkoutQueue;
        spawner.exitPoint = exitPoint;
        spawner.maxCustomers = 6;

        Day01CustomerDirector director = systems.AddComponent<Day01CustomerDirector>();
        director.mission = mission;
        director.spawner = spawner;
        director.totalCustomers = 5;
        director.spawnInterval = 3f;

        Day01TapFlowController tapFlow = systems.AddComponent<Day01TapFlowController>();
        tapFlow.gameplayCamera = gameplayCamera;
        tapFlow.cart = cart;
        tapFlow.shelf = shelf;
        tapFlow.mission = mission;

        Day01ScreenPresentation presentation = systems.AddComponent<Day01ScreenPresentation>();
        presentation.tapFlow = tapFlow;
        presentation.cart = cart;
        presentation.shelf = shelf;
        tapFlow.presentation = presentation;

        Day01HUD hud = systems.AddComponent<Day01HUD>();
        hud.mission = mission;
        hud.director = director;
        hud.tapFlow = tapFlow;

        Day01SaleFeedback saleFeedback = systems.AddComponent<Day01SaleFeedback>();
        saleFeedback.gameplayCamera = gameplayCamera;
        saleFeedback.checkoutAnchor = checkout.transform;
        saleFeedback.presentation = presentation;

        Debug.Log("Day01AutoInstaller: coherent screen-space management presentation active.");
    }

    static Camera ConfigureFixedCamera()
    {
        Camera main = Camera.main;
        if (main == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            main = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        CameraFollow oldFollow = main.GetComponent<CameraFollow>();
        if (oldFollow != null)
            oldFollow.enabled = false;

        StylizedCameraRig oldRig = main.GetComponent<StylizedCameraRig>();
        if (oldRig != null)
            oldRig.enabled = false;

        main.orthographic = true;
        main.orthographicSize = 10.2f;
        main.nearClipPlane = 0.1f;
        main.farClipPlane = 200f;
        main.clearFlags = CameraClearFlags.SolidColor;
        main.backgroundColor = new Color(0.06f, 0.08f, 0.09f);

        Vector3 focus = new Vector3(0f, 0.8f, 0.3f);
        main.transform.position = new Vector3(0f, 15.8f, -16.8f);
        main.transform.LookAt(focus);

        return main;
    }

    static GameObject CreateColliderRoot(
        string name,
        Vector3 position,
        Vector3 colliderSize,
        Vector3 colliderCenter)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = position;

        BoxCollider collider = obj.AddComponent<BoxCollider>();
        collider.size = colliderSize;
        collider.center = colliderCenter;
        return obj;
    }

    static Transform CreatePoint(string name, Vector3 position)
    {
        GameObject point = new GameObject(name);
        point.transform.position = position;
        return point.transform;
    }
}