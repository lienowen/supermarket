using UnityEngine;

public static class Day01DesignedDecorationBuilder
{
    public static int Build(Day01EnvironmentLayout layout)
    {
        ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
        if (catalog == null) return 0;

        GameObject root = new GameObject("DesignedDay01Decorations");
        int created = 0;

        if (catalog.fridgeDoubleDrinks != null)
        {
            DisableProceduralFridgeBank();

            GameObject fridgeA = DesignedArtIntegration.CreateFridgeDecoration(
                new Vector3(9.75f, 0f, -4.6f)
            );
            GameObject fridgeB = DesignedArtIntegration.CreateFridgeDecoration(
                new Vector3(9.75f, 0f, -0.8f)
            );

            created += ParentIfValid(fridgeA, root.transform);
            created += ParentIfValid(fridgeB, root.transform);
        }

        if (catalog.warehouseCorner != null)
        {
            GameObject warehouse = DesignedArtIntegration.CreateWarehouseCornerDecoration(
                new Vector3(-7.0f, 0f, -6.55f)
            );
            created += ParentIfValid(warehouse, root.transform);
        }

        if (catalog.palletBoxStack != null)
        {
            GameObject palletA = DesignedArtIntegration.CreatePalletStackDecoration(
                new Vector3(-4.7f, 0f, -5.7f)
            );
            GameObject palletB = DesignedArtIntegration.CreatePalletStackDecoration(
                new Vector3(-9.8f, 0f, -1.1f)
            );

            created += ParentIfValid(palletA, root.transform);
            created += ParentIfValid(palletB, root.transform);
        }

        if (catalog.promoStandSuperSale != null)
        {
            GameObject promo = DesignedArtIntegration.CreatePromoStandDecoration(
                new Vector3(6.7f, 0f, 0.9f)
            );
            created += ParentIfValid(promo, root.transform);
        }

        if (catalog.pottedPlantLarge != null)
        {
            GameObject plantA = DesignedArtIntegration.CreatePlantDecoration(
                new Vector3(9.0f, 0f, 6.45f)
            );
            GameObject plantB = DesignedArtIntegration.CreatePlantDecoration(
                new Vector3(-10.25f, 0f, 6.7f)
            );

            created += ParentIfValid(plantA, root.transform);
            created += ParentIfValid(plantB, root.transform);
        }

        if (created == 0)
        {
            Object.Destroy(root);
            Debug.Log("Day01DesignedDecorationBuilder: no optional Day01 cutouts found; fallback environment remains active.");
            return 0;
        }

        Debug.Log(
            "Day01DesignedDecorationBuilder: spawned " + created +
            " existing cutout decorations from Assets/Art."
        );
        return created;
    }

    private static int ParentIfValid(GameObject target, Transform parent)
    {
        if (target == null) return 0;

        target.transform.SetParent(parent, true);
        return 1;
    }

    private static void DisableProceduralFridgeBank()
    {
        for (int i = 0; i < 4; i++)
        {
            SetActiveIfFound("FridgeBody_" + i, false);
            SetActiveIfFound("FridgeDoor_" + i, false);
            SetActiveIfFound("FridgeLight_" + i, false);
        }

        SetActiveIfFound("ColdDrinksSign", false);
    }

    private static void SetActiveIfFound(string name, bool active)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
            obj.SetActive(active);
    }
}
