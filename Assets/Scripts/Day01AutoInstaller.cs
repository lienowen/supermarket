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

        GameObject ground = CreateGround();
        GameObject backWall;
        GameObject sideWall;
        CreateWarehouseShell(out backWall, out sideWall);
        Procedural3DVisualFactory.ApplyEnvironment(ground, backWall, sideWall);

        CreateStoreDecoration();

        GameObject player = CreatePlayer();
        CreateCamera(player.transform);

        for (int i = 0; i < 10; i++)
        {
            GameObject box = CreateCube(
                "DrinkBox_" + (i + 1),
                new Vector3(-7f + (i % 5) * 0.82f, 0.45f, -2.5f + (i / 5) * 0.92f),
                new Vector3(0.65f, 0.65f, 0.65f),
                new Color(0.85f, 0.15f, 0.12f)
            );

            ProductBox product = box.AddComponent<ProductBox>();
            product.productId = "cola_box";
            Procedural3DVisualFactory.ApplyDrinkBox(box);
        }

        GameObject cart = CreateCube(
            "ShoppingCart",
            new Vector3(-2.5f, 0.55f, 0f),
            new Vector3(1.5f, 1f, 1.1f),
            new Color(0.15f, 0.45f, 0.9f)
        );
        cart.AddComponent<CartSystem>();
        Procedural3DVisualFactory.ApplyCart(cart);

        GameObject shelf = CreateCube(
            "DrinkShelf",
            new Vector3(5.5f, 1.2f, 0f),
            new Vector3(2.2f, 2.4f, 1.2f),
            new Color(0.15f, 0.65f, 0.35f)
        );
        ShelfSystem shelfSystem = shelf.AddComponent<ShelfSystem>();
        shelfSystem.capacity = 10;
        shelfSystem.category = "drink";
        Procedural3DVisualFactory.ApplyShelf(shelf);

        Transform customerSpawn = CreatePoint("CustomerSpawn", new Vector3(-8f, 0f, 6f));
        Transform shelfPoint = CreatePoint("CustomerShelfPoint", new Vector3(4.2f, 0f, 0f));
        Transform checkoutPoint = CreatePoint("CustomerCheckoutPoint", new Vector3(6.3f, 0f, 4.5f));
        Transform exitPoint = CreatePoint("CustomerExitPoint", new Vector3(-8f, 0f, 6f));

        GameObject checkout = CreateCube(
            "CheckoutCounter",
            new Vector3(7.5f, 0.7f, 4.5f),
            new Vector3(2.2f, 1.4f, 1.2f),
            new Color(0.95f, 0.65f, 0.1f)
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

        Debug.Log("Day01AutoInstaller: animated procedural 3D Day01 generated successfully.");
    }

    static GameObject CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(-5f, 0f, 2f);

        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 2.2f;
        controller.radius = 0.45f;
        controller.center = new Vector3(0f, 1.1f, 0f);

        player.AddComponent<PlayerController>();
        player.AddComponent<InteractionSystem>();

        CarrySystem carry = player.AddComponent<CarrySystem>();
        GameObject carryPoint = new GameObject("CarryPoint");
        carryPoint.transform.SetParent(player.transform);
        carryPoint.transform.localPosition = new Vector3(0f, 1.25f, 0.9f);
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

        main.fieldOfView = 52f;
        main.nearClipPlane = 0.1f;
        main.farClipPlane = 150f;
        main.clearFlags = CameraClearFlags.SolidColor;
        main.backgroundColor = new Color(0.48f, 0.68f, 0.84f);
        main.transform.position = target.position + new Vector3(0f, 8.5f, -9.5f);

        CameraFollow follow = main.gameObject.GetComponent<CameraFollow>();
        if (follow == null)
            follow = main.gameObject.AddComponent<CameraFollow>();

        follow.target = target;
        follow.offset = new Vector3(0f, 8.5f, -9.5f);
        follow.smooth = 6f;
    }

    static GameObject CreateGround()
    {
        return CreateCube(
            "Ground",
            new Vector3(0f, -0.25f, 1f),
            new Vector3(22f, 0.5f, 16f),
            new Color(0.82f, 0.84f, 0.86f)
        );
    }

    static void CreateWarehouseShell(out GameObject backWall, out GameObject sideWall)
    {
        backWall = CreateCube(
            "WarehouseBackWall",
            new Vector3(-5f, 1.5f, -5f),
            new Vector3(8f, 3f, 0.3f),
            new Color(0.55f, 0.6f, 0.65f)
        );

        sideWall = CreateCube(
            "WarehouseSideWall",
            new Vector3(-9f, 1.5f, -1.5f),
            new Vector3(0.3f, 3f, 7f),
            new Color(0.55f, 0.6f, 0.65f)
        );
    }

    static void CreateStoreDecoration()
    {
        CreateCube(
            "StoreBackWall",
            new Vector3(6.5f, 1.8f, -5.5f),
            new Vector3(7f, 3.6f, 0.25f),
            new Color(0.88f, 0.91f, 0.93f)
        );

        CreateCube(
            "CheckoutZoneFloor",
            new Vector3(6.5f, 0.01f, 4.5f),
            new Vector3(6f, 0.04f, 3.2f),
            new Color(0.94f, 0.83f, 0.35f)
        );

        CreateCube(
            "WarehouseZoneFloor",
            new Vector3(-5.5f, 0.01f, -1.2f),
            new Vector3(6.5f, 0.04f, 6.8f),
            new Color(0.58f, 0.66f, 0.72f)
        );

        for (int i = 0; i < 4; i++)
        {
            CreateCube(
                "CeilingLight_" + i,
                new Vector3(-4f + i * 4f, 3.8f, 1f),
                new Vector3(2.2f, 0.08f, 0.25f),
                Color.white
            );
        }
    }

    static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Color color)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = color;

        return obj;
    }

    static Transform CreatePoint(string name, Vector3 position)
    {
        GameObject point = new GameObject(name);
        point.transform.position = position;
        return point.transform;
    }
}
