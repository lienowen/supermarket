using UnityEngine;
using UnityEngine.SceneManagement;

public static class Day01AutoInstaller
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "Day01") return;
        if (Object.FindObjectOfType<PlayerController>() != null) return;

        BuildPlayableDay01();
    }

    static void BuildPlayableDay01()
    {
        GameObject systems = new GameObject("GameSystems");

        systems.AddComponent<EconomySystem>();
        systems.AddComponent<StoreLevelSystem>();
        systems.AddComponent<ScoreSystem>();

        MissionSystem mission = systems.AddComponent<MissionSystem>();
        mission.targetAmount = 10;
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

        // EnvironmentBuilder owns all world surfaces and applies only dedicated safe textures.
        Day01EnvironmentLayout layout = Day01EnvironmentBuilder.Build();

        GameObject player = CreatePlayer(layout.playerSpawn);
        CreateCamera(player.transform);

        for (int i = 0; i < 10; i++)
        {
            int column = i % 5;
            int row = i / 5;
            Vector3 position = layout.boxOrigin + new Vector3(column * 0.92f, 0f, row * 0.82f);

            GameObject box = CreateColliderRoot(
                "DrinkBox_" + (i + 1),
                position,
                new Vector3(0.86f, 0.7f, 0.7f),
                new Vector3(0f, 0.35f, 0f)
            );

            ProductBox product = box.AddComponent<ProductBox>();
            product.productId = "cola_box";

            // Keep the object genuinely 3D, then add only a small safe front label from Assets/Art.
            Procedural3DVisualFactory.ApplyDrinkBox(box);
            DesignedArtIntegration.ApplyProduct(box, product.productId);
        }

        GameObject cart = CreateColliderRoot(
            "ShoppingCart",
            layout.cartPosition,
            new Vector3(1.25f, 1.9f, 1f),
            new Vector3(0f, 0.95f, 0f)
        );
        cart.AddComponent<CartSystem>();
        Procedural3DVisualFactory.ApplyCart(cart);

        GameObject shelf = CreateColliderRoot(
            "DrinkShelf",
            layout.shelfPosition,
            new Vector3(2.2f, 3.2f, 1f),
            new Vector3(0f, 1.6f, 0f)
        );
        ShelfSystem shelfSystem = shelf.AddComponent<ShelfSystem>();
        shelfSystem.capacity = 10;
        shelfSystem.category = "drink";
        Procedural3DVisualFactory.ApplyShelf(shelf);

        Transform customerSpawn = CreatePoint("CustomerSpawn", layout.customerSpawn);
        Transform shelfPoint = CreatePoint("CustomerShelfPoint", layout.customerShelfPoint);
        Transform checkoutPoint = CreatePoint("CustomerCheckoutPoint", layout.customerCheckoutPoint);
        Transform exitPoint = CreatePoint("CustomerExitPoint", layout.customerExitPoint);

        GameObject checkout = CreateColliderRoot(
            "CheckoutCounter",
            layout.checkoutPosition,
            new Vector3(2.5f, 2.9f, 1.2f),
            new Vector3(0f, 1.45f, 0f)
        );
        Procedural3DVisualFactory.ApplyCheckout(checkout);

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

        Day01HUD hud = systems.AddComponent<Day01HUD>();
        hud.mission = mission;
        hud.director = director;

        Debug.Log("Day01AutoInstaller: safe designed-art integration active. " + DesignedArtIntegration.GetBindingSummary());
    }

    static GameObject CreatePlayer(Vector3 position)
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = position;

        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 2.3f;
        controller.radius = 0.42f;
        controller.center = new Vector3(0f, 1.15f, 0f);

        player.AddComponent<PlayerController>();
        player.AddComponent<InteractionSystem>();

        CarrySystem carry = player.AddComponent<CarrySystem>();
        GameObject carryPoint = new GameObject("CarryPoint");
        carryPoint.transform.SetParent(player.transform);
        carryPoint.transform.localPosition = new Vector3(0f, 1.28f, 0.82f);
        carry.carryPoint = carryPoint.transform;

        Procedural3DVisualFactory.ApplyPlayer(player);
        return player;
    }

    static void CreateCamera(Transform target)
    {
        Camera main = Camera.main;
        if (main == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            main = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        main.fieldOfView = 46f;
        main.nearClipPlane = 0.1f;
        main.farClipPlane = 180f;
        main.clearFlags = CameraClearFlags.SolidColor;
        main.backgroundColor = new Color(0.48f, 0.65f, 0.78f);

        CameraFollow oldFollow = main.GetComponent<CameraFollow>();
        if (oldFollow != null)
            oldFollow.enabled = false;

        StylizedCameraRig rig = main.GetComponent<StylizedCameraRig>();
        if (rig == null)
            rig = main.gameObject.AddComponent<StylizedCameraRig>();

        rig.target = target;
        rig.targetOffset = new Vector3(0f, 1.1f, 0f);
        rig.distance = 10.5f;
        rig.height = 6.2f;
        rig.yaw = 28f;
        rig.positionSmooth = 7f;
        rig.rotationSmooth = 10f;

        Vector3 focus = target.position + rig.targetOffset;
        main.transform.position = focus + Quaternion.Euler(0f, rig.yaw, 0f) * new Vector3(0f, rig.height, -rig.distance);
        main.transform.LookAt(focus);
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
