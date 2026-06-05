using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
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
    private bool createdPanelInRefresh;

#if UNITY_EDITOR
    private bool editorPreviewQueued;
#endif

    private void Start()
    {
        RefreshSidebar();
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (!Application.isPlaying)
            QueueEditorPreview();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            QueueEditorPreview();
    }

    private void QueueEditorPreview()
    {
        if (editorPreviewQueued)
            return;

        editorPreviewQueued = true;
        UnityEditor.EditorApplication.delayCall += ApplyEditorPreview;
    }

    private void ApplyEditorPreview()
    {
        editorPreviewQueued = false;

        if (this == null || Application.isPlaying || !gameObject.scene.IsValid())
            return;

        RefreshSidebar();
        UnityEditor.EditorUtility.SetDirty(this);

        if (inventoryPanel != null)
            UnityEditor.EditorUtility.SetDirty(inventoryPanel.gameObject);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
#endif

    private void RefreshSidebar()
    {
        inventoryManager = InventoryManager.Instance != null
            ? InventoryManager.Instance
            : Application.isPlaying ? FindAnyObjectByType<InventoryManager>() : null;

        selectionManager = ItemSelectionManager.Instance != null
            ? ItemSelectionManager.Instance
            : Application.isPlaying ? FindAnyObjectByType<ItemSelectionManager>() : null;

        if (targetCanvas == null)
            targetCanvas = FindAnyObjectByType<Canvas>();

        AssignArielSceneFallbackSprites();
        createdPanelInRefresh = false;
        EnsurePanel();
        BuildSlots(createdPanelInRefresh);
    }

    public void Configure(ShelterItemData[] itemList, Sprite normalSlotSprite, Sprite selectedSlotSprite)
    {
        items = itemList;
        slotBackgroundSprite = normalSlotSprite;
        selectedSlotBackgroundSprite = selectedSlotSprite != null ? selectedSlotSprite : normalSlotSprite;
    }

    public void RefreshSidebarPreview()
    {
        RefreshSidebar();
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

        Transform existingPanel = targetCanvas.transform.Find("InventorySidebarPanel");
        if (existingPanel != null)
        {
            inventoryPanel = existingPanel.GetComponent<RectTransform>();
            return;
        }

        GameObject panelObject = new GameObject("InventorySidebarPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(targetCanvas.transform, false);
        panelObject.layer = targetCanvas.gameObject.layer;
        createdPanelInRefresh = true;

        inventoryPanel = panelObject.GetComponent<RectTransform>();
        Image panelImage = panelObject.GetComponent<Image>();
        panelImage.color = new Color(0.07f, 0.045f, 0.03f, 0.78f);
        panelImage.raycastTarget = false;
    }

    private void BuildSlots(bool applyDefaultPanelLayout)
    {
        if (inventoryPanel == null || items == null || items.Length == 0)
            return;

        if (applyDefaultPanelLayout)
        {
            inventoryPanel.anchorMin = new Vector2(0f, 0.5f);
            inventoryPanel.anchorMax = new Vector2(0f, 0.5f);
            inventoryPanel.pivot = new Vector2(0f, 0.5f);
            inventoryPanel.anchoredPosition = new Vector2(12f, 0f);
            inventoryPanel.sizeDelta = new Vector2(70f, 220f);
        }

        inventoryPanel.gameObject.layer = targetCanvas != null ? targetCanvas.gameObject.layer : inventoryPanel.gameObject.layer;

        for (int i = 0; i < items.Length; i++)
            EnsureSlot(items[i], i);
    }

    private void EnsureSlot(ShelterItemData itemData, int index)
    {
        if (itemData == null)
            return;

        inventoryManager?.RegisterItem(itemData);

        string slotName = itemData.DisplayName + "InventorySlot";
        Transform existingSlot = inventoryPanel.Find(slotName);
        bool createdSlot = existingSlot == null;
        GameObject slotObject = createdSlot
            ? new GameObject(slotName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(UIItemButton))
            : existingSlot.gameObject;

        if (createdSlot)
        {
            slotObject.transform.SetParent(inventoryPanel, false);
            slotObject.layer = inventoryPanel.gameObject.layer;

            RectTransform rect = slotObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(58f, 58f);
            rect.anchoredPosition = new Vector2(0f, -12f - (index * 68f));
        }

        Button button = slotObject.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SelectFromInventory(itemData));

        UIItemButton itemButton = slotObject.GetComponent<UIItemButton>();
        itemButton.SetLayoutMode(UIItemButtonLayout.SideSlot);
        itemButton.SetDisplayOptions(false, true, false);
        itemButton.SetBackgroundSprites(slotBackgroundSprite, selectedSlotBackgroundSprite);
        itemButton.Bind(itemData, inventoryManager, selectionManager);

        if (createdSlot)
            itemButton.ApplySideSlotLayoutPreset();
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
