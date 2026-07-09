using UnityEngine;
using UnityEngine.Rendering;

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
    private static Material cream;

    public static void ApplyPlayer(GameObject target)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        Transform torso = Part(PrimitiveType.Cube, "Torso", root, new Vector3(0f, 1.35f, 0f), new Vector3(0.72f, 0.86f, 0.42f), blue);
        Part(PrimitiveType.Cube, "Apron", root, new Vector3(0f, 1.32f, 0.225f), new Vector3(0.58f, 0.58f, 0.04f), darkBlue);
        Part(PrimitiveType.Sphere, "Head", root, new Vector3(0f, 2.08f, 0f), Vector3.one * 0.46f, skin);
        Part(PrimitiveType.Cube, "Cap", root, new Vector3(0f, 2.38f, -0.02f), new Vector3(0.54f, 0.14f, 0.52f), darkBlue);
        Part(PrimitiveType.Cube, "CapPeak", root, new Vector3(0f, 2.34f, 0.3f), new Vector3(0.42f, 0.07f, 0.24f), darkBlue);

        Transform leftArm = Part(PrimitiveType.Capsule, "LeftArm", root, new Vector3(-0.5f, 1.35f, 0f), new Vector3(0.17f, 0.42f, 0.17f), skin);
        Transform rightArm = Part(PrimitiveType.Capsule, "RightArm", root, new Vector3(0.5f, 1.35f, 0f), new Vector3(0.17f, 0.42f, 0.17f), skin);
        Transform leftLeg = Part(PrimitiveType.Cube, "LeftLeg", root, new Vector3(-0.19f, 0.55f, 0f), new Vector3(0.26f, 0.82f, 0.3f), darkBlue);
        Transform rightLeg = Part(PrimitiveType.Cube, "RightLeg", root, new Vector3(0.19f, 0.55f, 0f), new Vector3(0.26f, 0.82f, 0.3f), darkBlue);
        Part(PrimitiveType.Cube, "LeftShoe", root, new Vector3(-0.19f, 0.1f, 0.08f), new Vector3(0.3f, 0.16f, 0.46f), black);
        Part(PrimitiveType.Cube, "RightShoe", root, new Vector3(0.19f, 0.1f, 0.08f), new Vector3(0.3f, 0.16f, 0.46f), black);

        AddCharacterAnimator(root, leftArm, rightArm, leftLeg, rightLeg, torso);
    }

    public static void ApplyCustomer(GameObject target, int index)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        Material shirt = index % 4 == 0 ? red : index % 4 == 1 ? green : index % 4 == 2 ? yellow : blue;
        Material pants = index % 2 == 0 ? darkBlue : black;

        Transform torso = Part(PrimitiveType.Cube, "Torso", root, new Vector3(0f, 1.3f, 0f), new Vector3(0.7f, 0.82f, 0.42f), shirt);
        Part(PrimitiveType.Sphere, "Head", root, new Vector3(0f, 2.02f, 0f), Vector3.one * 0.46f, skin);
        Part(PrimitiveType.Cube, "Hair", root, new Vector3(0f, 2.3f, -0.04f), new Vector3(0.5f, 0.16f, 0.48f), index % 3 == 0 ? darkRed : black);

        Transform leftArm = Part(PrimitiveType.Capsule, "LeftArm", root, new Vector3(-0.49f, 1.3f, 0f), new Vector3(0.16f, 0.4f, 0.16f), skin);
        Transform rightArm = Part(PrimitiveType.Capsule, "RightArm", root, new Vector3(0.49f, 1.3f, 0f), new Vector3(0.16f, 0.4f, 0.16f), skin);
        Transform leftLeg = Part(PrimitiveType.Cube, "LeftLeg", root, new Vector3(-0.19f, 0.52f, 0f), new Vector3(0.25f, 0.78f, 0.3f), pants);
        Transform rightLeg = Part(PrimitiveType.Cube, "RightLeg", root, new Vector3(0.19f, 0.52f, 0f), new Vector3(0.25f, 0.78f, 0.3f), pants);
        Part(PrimitiveType.Cube, "LeftShoe", root, new Vector3(-0.19f, 0.09f, 0.08f), new Vector3(0.29f, 0.15f, 0.43f), black);
        Part(PrimitiveType.Cube, "RightShoe", root, new Vector3(0.19f, 0.09f, 0.08f), new Vector3(0.29f, 0.15f, 0.43f), black);

        AddCharacterAnimator(root, leftArm, rightArm, leftLeg, rightLeg, torso);
    }

    public static void ApplyDrinkBox(GameObject target)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        Part(PrimitiveType.Cube, "CrateBody", root, new Vector3(0f, 0.3f, 0f), new Vector3(0.84f, 0.52f, 0.64f), red);
        Part(PrimitiveType.Cube, "TopRail", root, new Vector3(0f, 0.59f, 0f), new Vector3(0.9f, 0.08f, 0.68f), darkRed);
        Part(PrimitiveType.Cube, "FrontPanel", root, new Vector3(0f, 0.32f, 0.335f), new Vector3(0.62f, 0.24f, 0.035f), white);
        Part(PrimitiveType.Cube, "Brand", root, new Vector3(0f, 0.32f, 0.36f), new Vector3(0.36f, 0.12f, 0.025f), darkRed);

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z += 2)
            {
                Transform lid = Part(
                    PrimitiveType.Cylinder,
                    "CanLid",
                    root,
                    new Vector3(x * 0.22f, 0.65f, z * 0.17f),
                    new Vector3(0.075f, 0.018f, 0.075f),
                    metal
                );
                lid.localRotation = Quaternion.identity;
            }
        }
    }

    public static void ApplyCart(GameObject target)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        Part(PrimitiveType.Cube, "LowerFrame", root, new Vector3(0f, 0.47f, 0f), new Vector3(1.05f, 0.09f, 0.72f), darkMetal);
        Part(PrimitiveType.Cube, "BasketFloor", root, new Vector3(0f, 0.86f, 0.06f), new Vector3(1.12f, 0.09f, 0.78f), metal);
        Part(PrimitiveType.Cube, "BasketFront", root, new Vector3(0f, 1.14f, 0.43f), new Vector3(1.12f, 0.58f, 0.08f), metal);
        Part(PrimitiveType.Cube, "BasketBack", root, new Vector3(0f, 1.14f, -0.31f), new Vector3(1.12f, 0.58f, 0.08f), metal);
        Part(PrimitiveType.Cube, "BasketLeft", root, new Vector3(-0.52f, 1.14f, 0.06f), new Vector3(0.08f, 0.58f, 0.72f), metal);
        Part(PrimitiveType.Cube, "BasketRight", root, new Vector3(0.52f, 1.14f, 0.06f), new Vector3(0.08f, 0.58f, 0.72f), metal);

        for (int i = -1; i <= 1; i++)
        {
            float y = 1.02f + i * 0.16f;
            Part(PrimitiveType.Cube, "FrontRail", root, new Vector3(0f, y, 0.475f), new Vector3(1.06f, 0.035f, 0.04f), darkMetal);
            Part(PrimitiveType.Cube, "BackRail", root, new Vector3(0f, y, -0.355f), new Vector3(1.06f, 0.035f, 0.04f), darkMetal);
        }

        Part(PrimitiveType.Cube, "HandleLeft", root, new Vector3(-0.44f, 1.55f, -0.55f), new Vector3(0.08f, 0.62f, 0.08f), darkMetal);
        Part(PrimitiveType.Cube, "HandleRight", root, new Vector3(0.44f, 1.55f, -0.55f), new Vector3(0.08f, 0.62f, 0.08f), darkMetal);
        Part(PrimitiveType.Cube, "HandleBar", root, new Vector3(0f, 1.83f, -0.55f), new Vector3(0.98f, 0.1f, 0.1f), blue);

        Wheel(root, new Vector3(-0.42f, 0.2f, 0.3f));
        Wheel(root, new Vector3(0.42f, 0.2f, 0.3f));
        Wheel(root, new Vector3(-0.42f, 0.2f, -0.3f));
        Wheel(root, new Vector3(0.42f, 0.2f, -0.3f));
    }

    public static void ApplyShelf(GameObject target)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        Part(PrimitiveType.Cube, "BackPanel", root, new Vector3(0f, 1.45f, -0.43f), new Vector3(2.05f, 2.75f, 0.08f), cream);

        for (int x = -1; x <= 1; x += 2)
        {
            Part(PrimitiveType.Cube, "Post", root, new Vector3(x * 0.94f, 1.4f, 0f), new Vector3(0.12f, 2.8f, 0.86f), darkMetal);
        }

        for (int level = 0; level < 4; level++)
        {
            float y = 0.25f + level * 0.72f;
            Part(PrimitiveType.Cube, "ShelfBoard", root, new Vector3(0f, y, 0f), new Vector3(2.02f, 0.1f, 0.88f), metal);

            if (level < 3)
            {
                for (int i = -3; i <= 3; i++)
                {
                    Material productMaterial = (i + level) % 3 == 0 ? red : (i + level) % 3 == 1 ? blue : yellow;
                    Part(
                        PrimitiveType.Cube,
                        "Product",
                        root,
                        new Vector3(i * 0.25f, y + 0.26f, 0.08f),
                        new Vector3(0.17f, 0.36f, 0.24f),
                        productMaterial
                    );
                }
            }
        }

        Part(PrimitiveType.Cube, "Header", root, new Vector3(0f, 3.02f, -0.02f), new Vector3(2.1f, 0.32f, 0.18f), green);
    }

    public static void ApplyCheckout(GameObject target)
    {
        EnsureMaterials();
        Transform root = PrepareRoot(target);

        Part(PrimitiveType.Cube, "CounterBase", root, new Vector3(0f, 0.52f, 0f), new Vector3(2.18f, 1f, 0.92f), wood);
        Part(PrimitiveType.Cube, "CounterTop", root, new Vector3(0f, 1.08f, 0f), new Vector3(2.32f, 0.14f, 1.02f), darkMetal);
        Part(PrimitiveType.Cube, "Belt", root, new Vector3(-0.4f, 1.18f, 0.02f), new Vector3(1.18f, 0.06f, 0.66f), black);
        Part(PrimitiveType.Cube, "Register", root, new Vector3(0.72f, 1.38f, -0.05f), new Vector3(0.42f, 0.36f, 0.34f), darkBlue);
        Part(PrimitiveType.Cube, "Screen", root, new Vector3(0.72f, 1.48f, 0.14f), new Vector3(0.28f, 0.2f, 0.04f), green);
        Part(PrimitiveType.Cube, "BagRack", root, new Vector3(1.28f, 0.76f, 0f), new Vector3(0.42f, 0.75f, 0.72f), cream);
        Part(PrimitiveType.Cube, "LanePost", root, new Vector3(0.96f, 2.02f, 0f), new Vector3(0.1f, 1.4f, 0.1f), darkMetal);
        Part(PrimitiveType.Cube, "OpenSign", root, new Vector3(0.96f, 2.66f, 0f), new Vector3(0.68f, 0.3f, 0.12f), green);
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

    private static Transform Part(
        PrimitiveType type,
        string name,
        Transform parent,
        Vector3 localPosition,
        Vector3 localScale,
        Material material)
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
        {
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.On;
            renderer.receiveShadows = true;
        }

        return part.transform;
    }

    private static void Wheel(Transform parent, Vector3 position)
    {
        Transform wheel = Part(
            PrimitiveType.Cylinder,
            "Wheel",
            parent,
            position,
            new Vector3(0.16f, 0.07f, 0.16f),
            black
        );
        wheel.localRotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private static void AddCharacterAnimator(
        Transform root,
        Transform leftArm,
        Transform rightArm,
        Transform leftLeg,
        Transform rightLeg,
        Transform body)
    {
        SimpleCharacterAnimator animator = root.gameObject.AddComponent<SimpleCharacterAnimator>();
        animator.leftArm = leftArm;
        animator.rightArm = rightArm;
        animator.leftLeg = leftLeg;
        animator.rightLeg = rightLeg;
        animator.body = body;
        animator.walkSwing = 24f;
        animator.walkFrequency = 7.5f;
        animator.idleBob = 0.018f;
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

    private static void EnsureMaterials()
    {
        if (blue != null) return;

        blue = Mat(new Color(0.1f, 0.42f, 0.82f), 0.25f);
        darkBlue = Mat(new Color(0.04f, 0.15f, 0.34f), 0.2f);
        skin = Mat(new Color(0.94f, 0.7f, 0.5f), 0.12f);
        black = Mat(new Color(0.05f, 0.06f, 0.08f), 0.12f);
        white = Mat(new Color(0.96f, 0.96f, 0.94f), 0.12f);
        red = Mat(new Color(0.86f, 0.08f, 0.08f), 0.18f);
        darkRed = Mat(new Color(0.4f, 0.02f, 0.02f), 0.12f);
        yellow = Mat(new Color(0.98f, 0.66f, 0.08f), 0.18f);
        green = Mat(new Color(0.12f, 0.56f, 0.26f), 0.16f);
        metal = Mat(new Color(0.48f, 0.56f, 0.64f), 0.55f);
        darkMetal = Mat(new Color(0.16f, 0.21f, 0.26f), 0.45f);
        wood = Mat(new Color(0.56f, 0.29f, 0.11f), 0.14f);
        cream = Mat(new Color(0.86f, 0.84f, 0.77f), 0.1f);
    }

    private static Material Mat(Color color, float smoothness)
    {
        Shader shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        Material material = new Material(shader);
        material.color = color;

        if (material.HasProperty("_Glossiness"))
            material.SetFloat("_Glossiness", smoothness);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);

        return material;
    }
}
