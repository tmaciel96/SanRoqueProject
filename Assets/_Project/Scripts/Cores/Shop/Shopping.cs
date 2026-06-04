using UnityEngine;
using UnityEngine.UI;

public class Shopping : MonoBehaviour
{
    private const int FoodCost = 250;
    private const int UpgradeCost = 500;
    private const int ToysCost = 150;

    [Header("Shop UI")]
    [SerializeField] private bool showPanelOnStart = false;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Image panelBackground;
    [SerializeField] private Sprite shopBackgroundSprite;
    [SerializeField] private Sprite itemButtonBackgroundSprite;
    [SerializeField] private Sprite selectedItemButtonBackgroundSprite;

    [Header("Items")]
    [SerializeField] private ShelterItemData foodItem;
    [SerializeField] private ShelterItemData upgradePenItem;
    [SerializeField] private ShelterItemData toysItem;

    [Header("Shop Buttons")]
    [SerializeField] private Button buyFoodButton;
    [SerializeField] private Button upgradePenButton;
    [SerializeField] private Button buyToysButton;
    [SerializeField] private Button closeButton;

    private Collider2D clickArea;
    private Camera mainCamera;
    private InventoryManager inventoryManager;
    private ItemSelectionManager selectionManager;

    private void Awake()
    {
        clickArea = GetComponent<Collider2D>();
        mainCamera = Camera.main;
        AutoAssignMissingReferences();
        AssignArielSceneFallbackSprites();

        if (!Application.isPlaying)
            return;

        EnsureCoreManagers();
        EnsureDefaultItems();
    }

    private void Start()
    {
        if (!Application.isPlaying)
            return;

        if (shopPanel == null)
        {
            Debug.LogWarning("Shopping: shopPanel no esta asignado en el inspector.", this);
        }

        LayoutShopPanel();
        BindItemButton(buyFoodButton, foodItem, "buyFoodButton");
        BindItemButton(upgradePenButton, upgradePenItem, "upgradePenButton");
        BindItemButton(buyToysButton, toysItem, "buyToysButton");
        AddButtonListener(closeButton, CloseShopMenu, "closeButton");

        if (showPanelOnStart)
            OpenShopMenu();
        else if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        if (!Input.GetMouseButtonDown(0) || clickArea == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (clickArea.OverlapPoint(worldPoint))
            OpenShopMenu();
    }

    private void OnMouseDown()
    {
        if (!Application.isPlaying)
            return;

        OpenShopMenu();
    }

    public void OpenShopMenu()
    {
        AutoAssignMissingReferences();

        if (shopPanel == null)
        {
            Debug.LogWarning("Shopping: no se puede abrir el panel porque shopPanel es null.", this);
            return;
        }

        if (panelBackground != null)
            panelBackground.gameObject.SetActive(shopBackgroundSprite != null);

        shopPanel.SetActive(true);
        shopPanel.transform.SetAsLastSibling();

        CanvasGroup canvasGroup = shopPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void CloseShopMenu()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    private void HandleItemPressed(ShelterItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("Shopping: el item del boton no esta configurado.", this);
            return;
        }

        EnsureCoreManagers();

        if (inventoryManager == null || selectionManager == null)
        {
            Debug.LogWarning("Shopping: falta InventoryManager o ItemSelectionManager en la escena.", this);
            return;
        }

        inventoryManager.TryBuyItem(itemData);
    }

    private void AutoAssignMissingReferences()
    {
        if (shopPanel == null)
        {
            Transform foundPanel = FindObjectInScene("shopPanel");
            if (foundPanel != null)
                shopPanel = foundPanel.gameObject;
        }

        if (panelBackground == null && shopPanel != null)
            panelBackground = shopPanel.transform.Find("BackgroundImage")?.GetComponent<Image>();

        if (buyFoodButton == null)
            buyFoodButton = FindObjectInScene("FoodButton")?.GetComponent<Button>();

        if (upgradePenButton == null)
            upgradePenButton = FindObjectInScene("UpgradePenButton")?.GetComponent<Button>();

        if (buyToysButton == null)
            buyToysButton = FindObjectInScene("ToysButton")?.GetComponent<Button>();

        if (closeButton == null)
            closeButton = FindObjectInScene("CloseButton")?.GetComponent<Button>();
    }

    private void AssignArielSceneFallbackSprites()
    {
#if UNITY_EDITOR
        if (gameObject.scene.name != "Ariel_Scene")
            return;

        Sprite shopBackground = LoadEditorSprite("Assets/_Project/Art/Sprites/Panel sin fondo sprite.png");
        Sprite buttonBackground = LoadEditorSprite("Assets/_Project/Art/Sprites/Botones sprite.png");

        if (shopBackground != null)
            shopBackgroundSprite = shopBackground;

        if (buttonBackground != null)
        {
            itemButtonBackgroundSprite = buttonBackground;
            selectedItemButtonBackgroundSprite = buttonBackground;
        }
#endif
    }

#if UNITY_EDITOR
    private static Sprite LoadEditorSprite(string assetPath)
    {
        foreach (UnityEngine.Object asset in UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath))
        {
            if (asset is Sprite sprite)
                return sprite;
        }

        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }
#endif

    private void AddButtonListener(Button button, UnityEngine.Events.UnityAction action, string fieldName)
    {
        if (button == null)
        {
            Debug.LogWarning($"Shopping: {fieldName} no esta asignado.", this);
            return;
        }

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    private void BindItemButton(Button button, ShelterItemData itemData, string fieldName)
    {
        AddButtonListener(button, () => HandleItemPressed(itemData), fieldName);

        if (button == null)
            return;

        UIItemButton itemButton = button.GetComponent<UIItemButton>();
        if (itemButton == null)
            itemButton = button.gameObject.AddComponent<UIItemButton>();

        itemButton.SetLayoutMode(UIItemButtonLayout.Row);
        itemButton.SetDisplayOptions(true, true, true);
        itemButton.SetBackgroundSprites(itemButtonBackgroundSprite, selectedItemButtonBackgroundSprite);
        itemButton.Bind(itemData, inventoryManager, selectionManager);
    }

    private void EnsureCoreManagers()
    {
        inventoryManager = InventoryManager.Instance != null
            ? InventoryManager.Instance
            : FindFirstObjectByType<InventoryManager>();

        if (inventoryManager == null)
        {
            GameObject inventoryObject = new GameObject("InventoryManager");
            inventoryManager = inventoryObject.AddComponent<InventoryManager>();
            Debug.Log("Shopping: se creo InventoryManager runtime para probar compras e inventario.", this);
        }

        selectionManager = ItemSelectionManager.Instance != null
            ? ItemSelectionManager.Instance
            : FindFirstObjectByType<ItemSelectionManager>();

        if (selectionManager == null)
        {
            GameObject selectionObject = new GameObject("ItemSelectionManager");
            selectionManager = selectionObject.AddComponent<ItemSelectionManager>();
            Debug.Log("Shopping: se creo ItemSelectionManager runtime para probar seleccion y aplicacion.", this);
        }
    }

    private void EnsureDefaultItems()
    {
        foodItem = foodItem != null
            ? foodItem
            : CreateRuntimeItem("Comida", FoodCost, 10, ShelterItemTargetType.Animal, hungerReduction: 20f, healthIncrease: 5f);

        upgradePenItem = upgradePenItem != null
            ? upgradePenItem
            : CreateRuntimeItem("Mejorar", UpgradeCost, 3, ShelterItemTargetType.Pen, penUpgradeAmount: 1);

        toysItem = toysItem != null
            ? toysItem
            : CreateRuntimeItem("Juguetitos", ToysCost, 10, ShelterItemTargetType.Animal, happinessIncrease: 20f);

        inventoryManager.RegisterItem(foodItem);
        inventoryManager.RegisterItem(upgradePenItem);
        inventoryManager.RegisterItem(toysItem);
    }

    private static ShelterItemData CreateRuntimeItem(
        string itemName,
        int itemCost,
        int maxInventory,
        ShelterItemTargetType targetType,
        float hungerReduction = 0f,
        float happinessIncrease = 0f,
        float healthIncrease = 0f,
        int penUpgradeAmount = 1)
    {
        ShelterItemData itemData = ScriptableObject.CreateInstance<ShelterItemData>();
        itemData.ConfigureRuntimeDefaults(itemName, itemCost, maxInventory, targetType, hungerReduction, happinessIncrease, healthIncrease, penUpgradeAmount);
        return itemData;
    }

    private void LayoutShopPanel()
    {
        if (shopPanel == null)
            return;

        bool preserveScenePanelLayout = gameObject.scene.name == "Ariel_Scene";

        RectTransform panelRect = shopPanel.GetComponent<RectTransform>();
        if (panelRect != null && !preserveScenePanelLayout)
        {
            panelRect.sizeDelta = new Vector2(640f, 330f);
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = new Vector2(170f, 10f);
        }

        Image panelImage = shopPanel.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = preserveScenePanelLayout ? Color.white : new Color(0.08f, 0.05f, 0.03f, 0.94f);

            if (preserveScenePanelLayout && shopBackgroundSprite != null)
            {
                panelImage.sprite = shopBackgroundSprite;
                panelImage.type = Image.Type.Simple;
            }
        }

        if (panelBackground != null)
        {
            panelBackground.sprite = shopBackgroundSprite;
            panelBackground.gameObject.SetActive(shopBackgroundSprite != null);
        }

        if (!preserveScenePanelLayout)
        {
            PositionButton(buyFoodButton, new Vector2(0f, 92f));
            PositionButton(upgradePenButton, new Vector2(0f, 0f));
            PositionButton(buyToysButton, new Vector2(0f, -92f));
            PositionCloseButton();
        }
    }

    private static void PositionButton(Button button, Vector2 anchoredPosition)
    {
        if (button == null)
            return;

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(560f, 74f);
        rect.localScale = new Vector3(1.0875f, 1f, 1f);
    }

    private void PositionCloseButton()
    {
        if (closeButton == null)
            return;

        RectTransform rect = closeButton.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-12f, -12f);
        rect.sizeDelta = new Vector2(42f, 30f);
    }

    private static Transform FindObjectInScene(string objectName)
    {
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform candidate in transforms)
        {
            if (candidate.name == objectName && candidate.gameObject.scene.IsValid())
                return candidate;
        }

        return null;
    }
}
