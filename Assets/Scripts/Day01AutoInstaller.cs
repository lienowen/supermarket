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

        EconomySystem economy = systems.AddComponent<EconomySystem>();
        StoreLevelSystem level = systems.AddComponent<StoreLevelSystem>();
        ScoreSystem score = systems.AddComponent<ScoreSystem>();
        MissionSystem mission = systems.AddComponent<MissionSystem>();
        mission.targetAmount = 10;
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

        CreateGround();
        CreateWarehouseShell();

        GameObject player = CreatePlayer();
        CreateCamera(player.transform);

        for (int i = 0; i < 10; i++)
        {
            GameObject box = CreateCube(
                "DrinkBox_" + (i + 1),
                new Vector3(-7f + (i % 5) * 0.8f, 0.45f, -2.5f + (i / 5) * 0.9f),
                new Vector3(0.65f, 0.65f, 0.65f),
                new Color(0.85f, 0.15f, 0.12f)
            );

            ProductBox product = box.AddComponent<ProductBox>();
            product.productId = "cola_box";
        }

        GameObject cart = CreateCube(
            "ShoppingCart",
            new Vector3(-2.5f, 0.55f, 0f),
            new Vector3(1.5f, 1f, 1.1f),
            new Color(0.15f, 0.45f, 0.9f)
        );
        cart.AddComponent<CartSystem>();

        GameObject shelf = CreateCube(
            "DrinkShelf",
            new Vector3(5.5f, 1.2f, 0f),
            new Vector3(2.2f, 2.4f, 1.2f),
            new Color(0.15f, 0.65f, 0.35f)
        );
        ShelfSystem shelfSystem = shelf.AddComponent<ShelfSystem>();
        shelfSystem.capacity = 10;
        shelfSystem.category = "drink";

        GameObject checkout = CreateCube(
            "CheckoutCounter",
            new Vector3(7.5f, 0.7f, 4.5f),
            new Vector3(2.2f, 1.4f, 1.2f),
            new Color(0.95f, 0.65f, 0.1f)
        );
        checkout.AddComponent<CheckoutSystem>();
        checkout.AddComponent<CheckoutQueueSystem>();

        Transform customerSpawn = CreatePoint("CustomerSpawn", new Vector3(-8f, 0f, 6f));
        Transform shelfPoint = CreatePoint("CustomerShelfPoint", new Vector3(4.2f, 0f, 0f));
        Transform checkoutPoint = CreatePoint("CustomerCheckoutPoint", new Vector3(6.3f, 0f, 4.5f));
        Transform exitPoint = CreatePoint("CustomerExitPoint", new Vector3(-8f, 0f, 6f));

        Day01CustomerDirector director = systems.AddComponent<Day01CustomerDirector>();
        director.mission = mission;
        director.spawnPoint = customerSpawn;
        director.shelfPoint = shelfPoint;
        director.checkoutPoint = checkoutPoint;
        director.exitPoint = exitPoint;

        Day01HUD hud = systems.AddComponent<Day01HUD>();
        hud.mission = mission;

        Debug.Log("Day01AutoInstaller: playable Day01 generated successfully.");
    }

    static GameObject CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(-5f, 1f, 2f);

        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 2f;
        controller.radius = 0.45f;

        player.AddComponent<PlayerController>();
        player.AddComponent<InteractionSystem>();

        CarrySystem carry = player.AddComponent<CarrySystem>();
        GameObject carryPoint = new GameObject("CarryPoint");
        carryPoint.transform.SetParent(player.transform);
        carryPoint.transform.localPosition = new Vector3(0f, 1.1f, 0.9f);
        carry.carryPoint = carryPoint.transform;

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        visual.name = "PlayerVisual";
        visual.transform.SetParent(player.transform);
        visual.transform.localPosition = Vector3.zero;
        Collider visualCollider = visual.GetComponent<Collider>();
        if (visualCollider != null)
            Object.Destroy(visualCollider);

        Renderer renderer = visual.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = new Color(0.2f, 0.55f, 0.95f);

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

        main.transform.position = target.position + new Vector3(0f, 8f, -8f);
        CameraFollow follow = main.gameObject.GetComponent<CameraFollow>();
        if (follow == null)
            follow = main.gameObject.AddComponent<CameraFollow>();

        follow.target = target;
        follow.offset = new Vector3(0f, 8f, -8f);
    }

    static void CreateGround()
    {
        CreateCube(
            "Ground",
            new Vector3(0f, -0.25f, 1f),
            new Vector3(22f, 0.5f, 16f),
            new Color(0.82f, 0.84f, 0.86f)
        );
    }

    static void CreateWarehouseShell()
    {
        CreateCube("WarehouseBackWall", new Vector3(-5f, 1.5f, -5f), new Vector3(8f, 3f, 0.3f), new Color(0.55f, 0.6f, 0.65f));
        CreateCube("WarehouseSideWall", new Vector3(-9f, 1.5f, -1.5f), new Vector3(0.3f, 3f, 7f), new Color(0.55f, 0.6f, 0.65f));
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
