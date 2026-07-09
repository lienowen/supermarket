using UnityEngine;

public static class Day01DesignedDecorationBuilder
{
    public static int Build(Day01EnvironmentLayout layout)
    {
        ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
        if (catalog == null) return 0;

        GameObject root = new GameObject("DesignedDay01Decorations");
        int created = 0;

        // One hero fridge is enough. The previous two billboard fridges competed with the
        // checkout area and made the scene look like a pile of rotating cards.
        if (catalog.fridgeDoubleDrinks != null)
        {
            DisableProceduralFridgeBank();

            GameObject fridge = DesignedArtIntegration.CreateFridgeDecoration(
                new Vector3(9.55f, 0f, -2.85f)
            );
            created += ParentIfValid(fridge, root.transform);
        }

        // Keep the warehouse corner as a single background anchor.
        if (catalog.warehouseCorner != null)
        {
            GameObject warehouse = DesignedArtIntegration.CreateWarehouseCornerDecoration(
                new Vector3(-7.4f, 0f, -6.35f)
            );
            created += ParentIfValid(warehouse, root.transform);
        }

        // Only one pallet stack; duplicate stacks were blocking the player's visual path.
        if (catalog.palletBoxStack != null)
        {
            GameObject pallet = DesignedArtIntegration.CreatePalletStackDecoration(
                new Vector3(-9.35f, 0f, -3.25f)
            );
            created += ParentIfValid(pallet, root.transform);
        }

        if (catalog.promoStandSuperSale != null)
        {
            GameObject promo = DesignedArtIntegration.CreatePromoStandDecoration(
                new Vector3(6.65f, 0f, 0.45f)
            );
            created += ParentIfValid(promo, root.transform);
        }

        // One plant near the entrance. A second mirrored plant added noise without gameplay value.
        if (catalog.pottedPlantLarge != null)
        {
            GameObject plant = DesignedArtIntegration.CreatePlantDecoration(
                new Vector3(-10.2f, 0f, 6.55f)
            );
            created += ParentIfValid(plant, root.transform);
        }

        if (created == 0)
        {
            Object.Destroy(root);
            Debug.Log("Day01DesignedDecorationBuilder: no optional Day01 cutouts found; fallback environment remains active.");
            return 0;
        }

        Debug.Log(
            "Day01DesignedDecorationBuilder: clean layout spawned " + created +
            " fixed world decorations from Assets/Art."
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
