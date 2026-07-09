using System.Collections.Generic;
using UnityEngine;

public class DesignedCharacterVisual : MonoBehaviour
{
    public enum CharacterRole
    {
        Player,
        Customer
    }

    [Header("Character")]
    public CharacterRole role = CharacterRole.Player;
    public int customerIndex;
    public float worldHeight = 2.55f;
    public float groundOffset = 0.02f;

    [Header("Walk Cycle")]
    public float walkSpeed = 10f;
    public float legSwingDegrees = 24f;
    public float bodyBobAmplitude = 0.055f;
    public float bodyLeanDegrees = 4f;
    public float movementThreshold = 0.015f;

    private const string VisualRootName = "DesignedCharacterVisual";
    private const string ProceduralRootName = "ProceduralVisual";

    private Transform visualRoot;
    private Transform upperBody;
    private Transform leftLeg;
    private Transform rightLeg;

    private SpriteRenderer upperRenderer;
    private SpriteRenderer leftLegRenderer;
    private SpriteRenderer rightLegRenderer;

    private CarrySystem carrySystem;
    private Sprite idleSprite;
    private Sprite carrySprite;
    private Sprite currentSprite;
    private Camera cachedCamera;

    private readonly List<Sprite> runtimeSprites = new List<Sprite>();

    private Vector3 previousPosition;
    private Vector3 planarVelocity;
    private float walkPhase;
    private float upperBaseY;
    private float hipY;
    private float horizontalFacing = 1f;

    public static bool ApplyPlayer(GameObject target)
    {
        if (target == null) return false;

        ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
        if (catalog == null) return false;

        Sprite sprite = catalog.playerIdle != null ? catalog.playerIdle : catalog.player;
        if (sprite == null) return false;

        DesignedCharacterVisual visual = target.GetComponent<DesignedCharacterVisual>();
        if (visual == null)
            visual = target.AddComponent<DesignedCharacterVisual>();

        visual.role = CharacterRole.Player;
        visual.worldHeight = 2.6f;
        visual.ConfigureNow();
        return true;
    }

    public static bool ApplyCustomer(GameObject target, int index)
    {
        if (target == null) return false;

        ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
        if (catalog == null || catalog.GetCustomer(index) == null) return false;

        DesignedCharacterVisual visual = target.GetComponent<DesignedCharacterVisual>();
        if (visual == null)
            visual = target.AddComponent<DesignedCharacterVisual>();

        visual.role = CharacterRole.Customer;
        visual.customerIndex = index;
        visual.worldHeight = 2.35f;
        visual.ConfigureNow();
        return true;
    }

    void Awake()
    {
        previousPosition = transform.position;
    }

    void Start()
    {
        ConfigureNow();
    }

    void Update()
    {
        UpdateMovementState();

        if (role == CharacterRole.Player)
            UpdatePlayerState();

        UpdateWalkAnimation();
    }

    void LateUpdate()
    {
        FaceCamera();
        UpdateHorizontalFacing();
    }

    void OnDestroy()
    {
        DestroyRuntimeSprites();
    }

    public void ConfigureNow()
    {
        ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
        if (catalog == null) return;

        if (role == CharacterRole.Player)
        {
            idleSprite = catalog.playerIdle != null ? catalog.playerIdle : catalog.player;
            carrySprite = catalog.playerCarry;
            carrySystem = GetComponent<CarrySystem>();
        }
        else
        {
            idleSprite = catalog.GetCustomer(customerIndex);
            carrySprite = null;
        }

        if (idleSprite == null) return;

        EnsureVisualRoot();
        HideProceduralVisual();
        ApplySprite(idleSprite);
        previousPosition = transform.position;
    }

    private void EnsureVisualRoot()
    {
        Transform existing = transform.Find(VisualRootName);
        if (existing != null)
            Destroy(existing.gameObject);

        GameObject root = new GameObject(VisualRootName);
        root.transform.SetParent(transform, false);
        visualRoot = root.transform;

        upperBody = CreatePart("UpperBody", 43);
        leftLeg = CreatePart("LeftLeg", 41);
        rightLeg = CreatePart("RightLeg", 42);

        upperRenderer = upperBody.GetComponent<SpriteRenderer>();
        leftLegRenderer = leftLeg.GetComponent<SpriteRenderer>();
        rightLegRenderer = rightLeg.GetComponent<SpriteRenderer>();
    }

    private Transform CreatePart(string name, int sortingOrder)
    {
        GameObject part = new GameObject(name);
        part.transform.SetParent(visualRoot, false);

        SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = sortingOrder;
        renderer.color = Color.white;

        return part.transform;
    }

    private void ApplySprite(Sprite sprite)
    {
        if (sprite == null || visualRoot == null) return;
        if (currentSprite == sprite && upperRenderer != null && upperRenderer.sprite != null) return;

        currentSprite = sprite;
        DestroyRuntimeSprites();

        Rect source = sprite.rect;
        float fullW = source.width;
        float fullH = source.height;

        // Use an overlapping articulated split. The overlap hides seams while allowing the
        // lower limbs to rotate independently, so the character no longer slides like a card.
        Rect upperRect = ClampRect(
            new Rect(
                source.x,
                source.y + fullH * 0.24f,
                fullW,
                fullH * 0.76f
            ),
            sprite.texture
        );

        Rect leftLegRect = ClampRect(
            new Rect(
                source.x,
                source.y,
                fullW * 0.58f,
                fullH * 0.46f
            ),
            sprite.texture
        );

        Rect rightLegRect = ClampRect(
            new Rect(
                source.x + fullW * 0.42f,
                source.y,
                fullW * 0.58f,
                fullH * 0.46f
            ),
            sprite.texture
        );

        float pixelsPerUnit = Mathf.Max(1f, fullH / Mathf.Max(0.1f, worldHeight));

        Sprite upperSprite = CreateRuntimeSprite(
            sprite.texture,
            upperRect,
            new Vector2(0.5f, 0f),
            pixelsPerUnit,
            sprite.name + "_upper"
        );
        Sprite leftSprite = CreateRuntimeSprite(
            sprite.texture,
            leftLegRect,
            new Vector2(0.5f, 1f),
            pixelsPerUnit,
            sprite.name + "_left_leg"
        );
        Sprite rightSprite = CreateRuntimeSprite(
            sprite.texture,
            rightLegRect,
            new Vector2(0.5f, 1f),
            pixelsPerUnit,
            sprite.name + "_right_leg"
        );

        upperRenderer.sprite = upperSprite;
        leftLegRenderer.sprite = leftSprite;
        rightLegRenderer.sprite = rightSprite;

        float worldWidth = worldHeight * (fullW / Mathf.Max(1f, fullH));
        upperBaseY = groundOffset + worldHeight * 0.24f;
        hipY = groundOffset + worldHeight * 0.43f;

        upperBody.localPosition = new Vector3(0f, upperBaseY, 0f);
        leftLeg.localPosition = new Vector3(-worldWidth * 0.115f, hipY, 0.012f);
        rightLeg.localPosition = new Vector3(worldWidth * 0.115f, hipY, 0.006f);

        upperBody.localRotation = Quaternion.identity;
        leftLeg.localRotation = Quaternion.identity;
        rightLeg.localRotation = Quaternion.identity;
        visualRoot.localScale = Vector3.one;
    }

    private void UpdatePlayerState()
    {
        if (carrySystem == null)
            carrySystem = GetComponent<CarrySystem>();

        bool carrying = carrySystem != null && carrySystem.HasItem();
        Sprite desired = carrying && carrySprite != null ? carrySprite : idleSprite;

        if (desired != null && desired != currentSprite)
            ApplySprite(desired);
    }

    private void UpdateMovementState()
    {
        Vector3 current = transform.position;
        Vector3 delta = current - previousPosition;
        delta.y = 0f;
        previousPosition = current;

        float dt = Mathf.Max(0.0001f, Time.deltaTime);
        planarVelocity = delta / dt;
    }

    private void UpdateWalkAnimation()
    {
        if (visualRoot == null || upperBody == null || leftLeg == null || rightLeg == null)
            return;

        float speed = planarVelocity.magnitude;
        bool moving = speed > movementThreshold;

        if (!moving)
        {
            walkPhase = Mathf.Lerp(walkPhase, 0f, 1f - Mathf.Exp(-8f * Time.deltaTime));
            upperBody.localPosition = Vector3.Lerp(
                upperBody.localPosition,
                new Vector3(0f, upperBaseY, 0f),
                1f - Mathf.Exp(-14f * Time.deltaTime)
            );
            upperBody.localRotation = Quaternion.Slerp(
                upperBody.localRotation,
                Quaternion.identity,
                1f - Mathf.Exp(-14f * Time.deltaTime)
            );
            leftLeg.localRotation = Quaternion.Slerp(
                leftLeg.localRotation,
                Quaternion.identity,
                1f - Mathf.Exp(-16f * Time.deltaTime)
            );
            rightLeg.localRotation = Quaternion.Slerp(
                rightLeg.localRotation,
                Quaternion.identity,
                1f - Mathf.Exp(-16f * Time.deltaTime)
            );
            return;
        }

        float speedFactor = Mathf.Clamp(speed / 5f, 0.65f, 1.45f);
        walkPhase += Time.deltaTime * walkSpeed * speedFactor;

        float swing = Mathf.Sin(walkPhase) * legSwingDegrees;
        float bob = Mathf.Abs(Mathf.Sin(walkPhase)) * bodyBobAmplitude;
        float lean = Mathf.Sin(walkPhase * 0.5f) * bodyLeanDegrees;

        leftLeg.localRotation = Quaternion.Euler(0f, 0f, swing);
        rightLeg.localRotation = Quaternion.Euler(0f, 0f, -swing);
        upperBody.localPosition = new Vector3(0f, upperBaseY + bob, 0f);
        upperBody.localRotation = Quaternion.Euler(0f, 0f, lean);
    }

    private void FaceCamera()
    {
        if (visualRoot == null) return;

        if (cachedCamera == null)
            cachedCamera = Camera.main;
        if (cachedCamera == null) return;

        Vector3 direction = cachedCamera.transform.position - visualRoot.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        visualRoot.rotation = Quaternion.LookRotation(-direction.normalized, Vector3.up);
    }

    private void UpdateHorizontalFacing()
    {
        if (visualRoot == null || cachedCamera == null) return;
        if (planarVelocity.sqrMagnitude < movementThreshold * movementThreshold) return;

        float horizontal = Vector3.Dot(planarVelocity.normalized, cachedCamera.transform.right);
        if (Mathf.Abs(horizontal) < 0.12f) return;

        horizontalFacing = horizontal >= 0f ? 1f : -1f;
        visualRoot.localScale = new Vector3(horizontalFacing, 1f, 1f);
    }

    private void HideProceduralVisual()
    {
        Transform procedural = transform.Find(ProceduralRootName);
        if (procedural != null)
            procedural.gameObject.SetActive(false);
    }

    private Sprite CreateRuntimeSprite(
        Texture2D texture,
        Rect rect,
        Vector2 pivot,
        float pixelsPerUnit,
        string name)
    {
        Sprite result = Sprite.Create(
            texture,
            rect,
            pivot,
            pixelsPerUnit,
            0,
            SpriteMeshType.FullRect
        );
        result.name = name;
        runtimeSprites.Add(result);
        return result;
    }

    private Rect ClampRect(Rect rect, Texture2D texture)
    {
        float x = Mathf.Clamp(rect.x, 0f, texture.width - 1f);
        float y = Mathf.Clamp(rect.y, 0f, texture.height - 1f);
        float width = Mathf.Clamp(rect.width, 1f, texture.width - x);
        float height = Mathf.Clamp(rect.height, 1f, texture.height - y);
        return new Rect(x, y, width, height);
    }

    private void DestroyRuntimeSprites()
    {
        for (int i = 0; i < runtimeSprites.Count; i++)
        {
            if (runtimeSprites[i] != null)
                Destroy(runtimeSprites[i]);
        }
        runtimeSprites.Clear();
    }
}
