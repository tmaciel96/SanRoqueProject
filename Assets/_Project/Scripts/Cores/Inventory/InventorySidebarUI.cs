using UnityEngine;
using UnityEngine.UI;

public class InventorySidebarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private Sprite slotBackgroundSprite;
    [SerializeField] private Sprite selectedSlotBackgroundSprite;

    [Header("Items")]
    [SerializeField] private ShelterItemData[] items;

    private InventoryManager inventoryManager;
    private ItemSelectionManager selectionManager;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance != null
            ? InventoryManager.Instance
            : FindFirstObjectByType<InventoryManager>();

        selectionManager = ItemSelectionManager.Instance != null
            ? ItemSelectionManager.Instance
            : FindFirstObjectByType<ItemSelectionManager>();

        if (targetCanvas == null)
            targetCanvas = FindFirstObjectByType<Canvas>();

        AssignArielSceneFallbackSprites();
        EnsurePanel();
        BuildSlots();
    }

    public void Configure(ShelterItemData[] itemList, Sprite normalSlotSprite, Sprite selectedSlotSprite)
    {
        items = itemList;
        slotBackgroundSprite = normalSlotSprite;
        selectedSlotBackgroundSprite = selectedSlotSprite != null ? selectedSlotSprite : normalSlotSprite;
    }

    private void AssignArielSceneFallbackSprites()
    {
#if UNITY_EDITOR
        if (gameObject.scene.name != "Ariel_Scene")
            return;

        Sprite buttonBackground = LoadEditorSprite("Assets/_Project/Art/Sprites/Botones sprite.png");
        if (buttonBackground == null)
            return;

        slotBackgroundSprite = buttonBackground;
        selectedSlotBackgroundSprite = buttonBackground;
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

    private void EnsurePanel()
    {
        if (inventoryPanel != null)
            return;

        if (targetCanvas == null)
        {
            Debug.LogWarning("InventorySidebarUI: no encontre Canvas para crear el inventario lateral.", this);
            return;
        }

        GameObject panelObject = new GameObject("InventorySidebarPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(targetCanvas.transform, false);

        inventoryPanel = panelObject.GetComponent<RectTransform>();
        Image panelImage = panelObject.GetComponent<Image>();
        panelImage.color = new Color(0.07f, 0.045f, 0.03f, 0.78f);
        panelImage.raycastTarget = false;
    }

    private void BuildSlots()
    {
        if (inventoryPanel == null || items == null || items.Length == 0)
            return;

        inventoryPanel.anchorMin = new Vector2(0f, 0.5f);
        inventoryPanel.anchorMax = new Vector2(0f, 0.5f);
        inventoryPanel.pivot = new Vector2(0f, 0.5f);
        inventoryPanel.anchoredPosition = new Vector2(8f, 0f);
        inventoryPanel.sizeDelta = new Vector2(82f, 246f);

        for (int i = inventoryPanel.childCount - 1; i >= 0; i--)
            Destroy(inventoryPanel.GetChild(i).gameObject);

        for (int i = 0; i < items.Length; i++)
            CreateSlot(items[i], i);
    }

    private void CreateSlot(ShelterItemData itemData, int index)
    {
        if (itemData == null)
            return;

        inventoryManager?.RegisterItem(itemData);

        GameObject slotObject = new GameObject(itemData.DisplayName + "InventorySlot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(UIItemButton));
        slotObject.transform.SetParent(inventoryPanel, false);

        RectTransform rect = slotObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(66f, 66f);
        rect.anchoredPosition = new Vector2(0f, -10f - (index * 76f));

        Button button = slotObject.GetComponent<Button>();
        button.onClick.AddListener(() => SelectFromInventory(itemData));

        UIItemButton itemButton = slotObject.GetComponent<UIItemButton>();
        itemButton.SetLayoutMode(UIItemButtonLayout.SideSlot);
        itemButton.SetDisplayOptions(false, true, false);
        itemButton.SetBackgroundSprites(slotBackgroundSprite, selectedSlotBackgroundSprite);
        itemButton.Bind(itemData, inventoryManager, selectionManager);
    }

    private void SelectFromInventory(ShelterItemData itemData)
    {
        if (selectionManager == null)
            selectionManager = ItemSelectionManager.Instance;

        if (selectionManager == null)
        {
            Debug.LogWarning("InventorySidebarUI: falta ItemSelectionManager para seleccionar item.", this);
            return;
        }

        selectionManager.SelectItem(itemData);
    }
}
