using UnityEngine;

public static class Procedural3DVisualFactory
{
    private const string VisualRootName = "ProceduralVisual";

    private static Material blue;
    private static Material darkBlue;
    private static Material skin;
    private static Material black;
    private static Material white;
    private static Material red;
    private static Material darkRed;
    private static Material yellow;
    private static Material green;
    private static Material metal;
    private static Material darkMetal;
    private static Material wood;
    private static Material floor;
    private static Material wall;

    public static void ApplyEnvironment(GameObject ground, GameObject backWall, GameObject sideWall)
    {
        EnsureMaterials();
        SetRootMaterial(ground, floor);
        SetRootMaterial(backWall, wall);
        SetRootMaterial(sideWall, wall);
    }

    public static void ApplyPlayer(GameObject target)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        Transform body = Part(PrimitiveType.Capsule, "Body", root, new Vector3(0f, 1.1f, 0f), new Vector3(0.7f, 0.65f, 0.5f), blue);
        Transform head = Part(PrimitiveType.Sphere, "Head", root, new Vector3(0f, 2.05f, 0f), Vector3.one * 0.55f, skin);
        Part(PrimitiveType.Cube, "Cap", root, new Vector3(0f, 2.42f, 0f), new Vector3(0.62f, 0.16f, 0.62f), darkBlue);
        Part(PrimitiveType.Cube, "CapPeak", root, new Vector3(0f, 2.38f, 0.34f), new Vector3(0.52f, 0.08f, 0.28f), darkBlue);

        Transform leftArm = Part(PrimitiveType.Cube, "LeftArm", root, new Vector3(-0.52f, 1.25f, 0f), new Vector3(0.22f, 0.85f, 0.24f), skin);
        Transform rightArm = Part(PrimitiveType.Cube, "RightArm", root, new Vector3(0.52f, 1.25f, 0f), new Vector3(0.22f, 0.85f, 0.24f), skin);
        Transform leftLeg = Part(PrimitiveType.Cube, "LeftLeg", root, new Vector3(-0.2f, 0.3f, 0f), new Vector3(0.28f, 0.9f, 0.32f), black);
        Transform rightLeg = Part(PrimitiveType.Cube, "RightLeg", root, new Vector3(0.2f, 0.3f, 0f), new Vector3(0.28f, 0.9f, 0.32f), black);

        AddCharacterAnimator(root, leftArm, rightArm, leftLeg, rightLeg, body);
        head.localScale = Vector3.one * 0.55f;
    }

    public static void ApplyCustomer(GameObject target, int index)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        Material shirt = index % 4 == 0 ? red : index % 4 == 1 ? green : index % 4 == 2 ? yellow : blue;
        Material pants = index % 2 == 0 ? darkBlue : black;

        Transform body = Part(PrimitiveType.Capsule, "Body", root, new Vector3(0f, 1.05f, 0f), new Vector3(0.72f, 0.62f, 0.5f), shirt);
        Part(PrimitiveType.Sphere, "Head", root, new Vector3(0f, 1.98f, 0f), Vector3.one * 0.55f, skin);
        Part(PrimitiveType.Cube, "Hair", root, new Vector3(0f, 2.28f, -0.02f), new Vector3(0.56f, 0.18f, 0.52f), black);

        Transform leftArm = Part(PrimitiveType.Cube, "LeftArm", root, new Vector3(-0.52f, 1.2f, 0f), new Vector3(0.22f, 0.82f, 0.24f), skin);
        Transform rightArm = Part(PrimitiveType.Cube, "RightArm", root, new Vector3(0.52f, 1.2f, 0f), new Vector3(0.22f, 0.82f, 0.24f), skin);
        Transform leftLeg = Part(PrimitiveType.Cube, "LeftLeg", root, new Vector3(-0.2f, 0.28f, 0f), new Vector3(0.28f, 0.88f, 0.32f), pants);
        Transform rightLeg = Part(PrimitiveType.Cube, "RightLeg", root, new Vector3(0.2f, 0.28f, 0f), new Vector3(0.28f, 0.88f, 0.32f), pants);

        AddCharacterAnimator(root, leftArm, rightArm, leftLeg, rightLeg, body);
    }

    public static void ApplyDrinkBox(GameObject target)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        Part(PrimitiveType.Cube, "CrateBody", root, new Vector3(0f, 0.35f, 0f), new Vector3(0.9f, 0.62f, 0.72f), red);
        Part(PrimitiveType.Cube, "FrontBand", root, new Vector3(0f, 0.36f, 0.37f), new Vector3(0.7f, 0.22f, 0.03f), white);
        Part(PrimitiveType.Cube, "Label", root, new Vector3(0f, 0.36f, 0.395f), new Vector3(0.4f, 0.12f, 0.02f), darkRed);

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z += 2)
            {
                Transform can = Part(PrimitiveType.Cylinder, "Can", root, new Vector3(x * 0.22f, 0.72f, z * 0.18f), new Vector3(0.1f, 0.18f, 0.1f), red);
                can.localRotation = Quaternion.identity;
            }
        }
    }

    public static void ApplyCart(GameObject target)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        Part(PrimitiveType.Cube, "BasketBottom", root, new Vector3(0f, 0.78f, 0.05f), new Vector3(1.25f, 0.12f, 0.9f), metal);
        Part(PrimitiveType.Cube, "BasketFront", root, new Vector3(0f, 1.12f, 0.47f), new Vector3(1.25f, 0.72f, 0.1f), metal);
        Part(PrimitiveType.Cube, "BasketBack", root, new Vector3(0f, 1.12f, -0.38f), new Vector3(1.25f, 0.72f, 0.1f), metal);
        Part(PrimitiveType.Cube, "BasketLeft", root, new Vector3(-0.58f, 1.12f, 0.05f), new Vector3(0.1f, 0.72f, 0.85f), metal);
        Part(PrimitiveType.Cube, "BasketRight", root, new Vector3(0.58f, 1.12f, 0.05f), new Vector3(0.1f, 0.72f, 0.85f), metal);

        Part(PrimitiveType.Cube, "HandleLeft", root, new Vector3(-0.47f, 1.62f, -0.66f), new Vector3(0.1f, 0.7f, 0.1f), darkMetal);
        Part(PrimitiveType.Cube, "HandleRight", root, new Vector3(0.47f, 1.62f, -0.66f), new Vector3(0.1f, 0.7f, 0.1f), darkMetal);
        Part(PrimitiveType.Cube, "HandleBar", root, new Vector3(0f, 1.92f, -0.66f), new Vector3(1.05f, 0.12f, 0.12f), blue);

        Wheel(root, new Vector3(-0.47f, 0.32f, 0.35f));
        Wheel(root, new Vector3(0.47f, 0.32f, 0.35f));
        Wheel(root, new Vector3(-0.47f, 0.32f, -0.35f));
        Wheel(root, new Vector3(0.47f, 0.32f, -0.35f));
    }

    public static void ApplyShelf(GameObject target)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        for (int x = -1; x <= 1; x += 2)
        {
            Part(PrimitiveType.Cube, "Post", root, new Vector3(x * 0.95f, 1.35f, 0f), new Vector3(0.14f, 2.7f, 0.82f), darkMetal);
        }

        for (int level = 0; level < 4; level++)
        {
            float y = 0.25f + level * 0.72f;
            Part(PrimitiveType.Cube, "ShelfBoard", root, new Vector3(0f, y, 0f), new Vector3(2.05f, 0.12f, 0.9f), metal);

            if (level < 3)
            {
                for (int i = -3; i <= 3; i++)
                {
                    Part(PrimitiveType.Cube, "Product", root, new Vector3(i * 0.25f, y + 0.3f, 0.05f), new Vector3(0.18f, 0.42f, 0.28f), i % 2 == 0 ? red : blue);
                }
            }
        }

        Part(PrimitiveType.Cube, "Header", root, new Vector3(0f, 3.05f, 0f), new Vector3(2.1f, 0.38f, 0.18f), green);
    }

    public static void ApplyCheckout(GameObject target)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        Part(PrimitiveType.Cube, "CounterBase", root, new Vector3(0f, 0.58f, 0f), new Vector3(2.3f, 1.1f, 1f), wood);
        Part(PrimitiveType.Cube, "CounterTop", root, new Vector3(0f, 1.18f, 0f), new Vector3(2.45f, 0.16f, 1.08f), darkMetal);
        Part(PrimitiveType.Cube, "Belt", root, new Vector3(-0.42f, 1.29f, 0f), new Vector3(1.15f, 0.08f, 0.72f), black);
        Part(PrimitiveType.Cube, "Register", root, new Vector3(0.72f, 1.48f, -0.08f), new Vector3(0.48f, 0.42f, 0.4f), darkBlue);
        Part(PrimitiveType.Cube, "Screen", root, new Vector3(0.72f, 1.58f, 0.16f), new Vector3(0.32f, 0.22f, 0.05f), green);
        Part(PrimitiveType.Cube, "LaneSign", root, new Vector3(0.95f, 2.15f, 0f), new Vector3(0.12f, 1.3f, 0.12f), darkMetal);
        Part(PrimitiveType.Cube, "OpenSign", root, new Vector3(0.95f, 2.72f, 0f), new Vector3(0.72f, 0.36f, 0.12f), green);
    }

    private static Transform PrepareRoot(GameObject target)
    {
        if (target == null) return null;

        MeshRenderer rootRenderer = target.GetComponent<MeshRenderer>();
        if (rootRenderer != null)
            rootRenderer.enabled = false;

        Transform oldSprite = target.transform.Find("ArtVisual");
        if (oldSprite != null)
            Object.Destroy(oldSprite.gameObject);

        Transform existing = target.transform.Find(VisualRootName);
        if (existing != null)
            Object.Destroy(existing.gameObject);

        GameObject rootObject = new GameObject(VisualRootName);
        Transform root = rootObject.transform;
        root.SetParent(target.transform, false);
        root.localPosition = Vector3.zero;
        root.localRotation = Quaternion.identity;
        root.localScale = InverseAbsScale(target.transform.lossyScale);
        return root;
    }

    private static Transform Part(PrimitiveType type, string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPosition;
        part.transform.localRotation = Quaternion.identity;
        part.transform.localScale = localScale;

        Collider collider = part.GetComponent<Collider>();
        if (collider != null)
            Object.Destroy(collider);

        Renderer renderer = part.GetComponent<Renderer>();
        if (renderer != null)
            renderer.sharedMaterial = material;

        return part.transform;
    }

    private static void Wheel(Transform parent, Vector3 position)
    {
        Transform wheel = Part(PrimitiveType.Cylinder, "Wheel", parent, position, new Vector3(0.18f, 0.08f, 0.18f), black);
        wheel.localRotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private static void AddCharacterAnimator(Transform root, Transform leftArm, Transform rightArm, Transform leftLeg, Transform rightLeg, Transform body)
    {
        SimpleCharacterAnimator animator = root.gameObject.AddComponent<SimpleCharacterAnimator>();
        animator.leftArm = leftArm;
        animator.rightArm = rightArm;
        animator.leftLeg = leftLeg;
        animator.rightLeg = rightLeg;
        animator.body = body;
    }

    private static Vector3 InverseAbsScale(Vector3 scale)
    {
        return new Vector3(Inverse(scale.x), Inverse(scale.y), Inverse(scale.z));
    }

    private static float Inverse(float value)
    {
        float magnitude = Mathf.Abs(value);
        return magnitude < 0.001f ? 1f : 1f / magnitude;
    }

    private static void SetRootMaterial(GameObject target, Material material)
    {
        if (target == null) return;

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = true;
            renderer.sharedMaterial = material;
        }

        Transform artVisual = target.transform.Find("ArtVisual");
        if (artVisual != null)
            Object.Destroy(artVisual.gameObject);
    }

    private static void EnsureMaterials()
    {
        if (blue != null) return;

        blue = Mat(new Color(0.12f, 0.48f, 0.95f), 0.25f);
        darkBlue = Mat(new Color(0.05f, 0.18f, 0.42f), 0.2f);
        skin = Mat(new Color(0.96f, 0.72f, 0.52f), 0.15f);
        black = Mat(new Color(0.06f, 0.07f, 0.09f), 0.1f);
        white = Mat(new Color(0.95f, 0.96f, 0.98f), 0.15f);
        red = Mat(new Color(0.9f, 0.08f, 0.08f), 0.2f);
        darkRed = Mat(new Color(0.45f, 0.02f, 0.02f), 0.15f);
        yellow = Mat(new Color(1f, 0.65f, 0.05f), 0.2f);
        green = Mat(new Color(0.1f, 0.62f, 0.26f), 0.18f);
        metal = Mat(new Color(0.55f, 0.62f, 0.7f), 0.7f);
        darkMetal = Mat(new Color(0.18f, 0.24f, 0.3f), 0.55f);
        wood = Mat(new Color(0.62f, 0.32f, 0.12f), 0.15f);
        floor = Mat(new Color(0.78f, 0.83f, 0.86f), 0.08f);
        wall = Mat(new Color(0.62f, 0.68f, 0.74f), 0.12f);
    }

    private static Material Mat(Color color, float smoothness)
    {
        Shader shader = Shader.Find("Standard");
        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        Material material = new Material(shader);
        material.color = color;

        if (material.HasProperty("_Glossiness"))
            material.SetFloat("_Glossiness", smoothness);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);

        return material;
    }
}
