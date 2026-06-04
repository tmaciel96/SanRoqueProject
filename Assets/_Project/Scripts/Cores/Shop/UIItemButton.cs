using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UIItemButtonLayout
{
    Row,
    SideSlot
}

[RequireComponent(typeof(Button))]
public class UIItemButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("Visuals")]
    [SerializeField] private Sprite normalBackgroundSprite;
    [SerializeField] private Sprite selectedBackgroundSprite;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private UIItemButtonLayout layoutMode = UIItemButtonLayout.SideSlot;
    [SerializeField] private bool showCost = true;
    [SerializeField] private bool showCount = true;
    [SerializeField] private bool showMaxCount = true;

    private Button button;
    private Image background;
    private ShelterItemData itemData;
    private InventoryManager inventoryManager;
    private ItemSelectionManager selectionManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        background = GetComponent<Image>();
        EnsureVisualChildren();
    }

    private void OnDestroy()
    {
        if (inventoryManager != null)
            inventoryManager.Changed -= Refresh;

        if (selectionManager != null)
            selectionManager.SelectionChanged -= OnSelectionChanged;
    }

    public void Bind(ShelterItemData item, InventoryManager inventory, ItemSelectionManager selection)
    {
        if (button == null)
            button = GetComponent<Button>();

        if (background == null)
            background = GetComponent<Image>();

        EnsureVisualChildren();

        itemData = item;
        inventoryManager = inventory;
        selectionManager = selection;

        if (inventoryManager != null)
            inventoryManager.Changed += Refresh;

        if (selectionManager != null)
            selectionManager.SelectionChanged += OnSelectionChanged;

        Refresh();
    }

    public void SetLayoutMode(UIItemButtonLayout mode)
    {
        layoutMode = mode;
        EnsureVisualChildren();
        Refresh();
    }

    public void SetDisplayOptions(bool shouldShowCost, bool shouldShowCount, bool shouldShowMaxCount)
    {
        showCost = shouldShowCost;
        showCount = shouldShowCount;
        showMaxCount = shouldShowMaxCount;
        Refresh();
    }

    public void Refresh()
    {
        if (itemData == null)
            return;

        if (labelText != null)
        {
            labelText.text = itemData.DisplayName;
            labelText.gameObject.SetActive(layoutMode == UIItemButtonLayout.Row);
        }

        if (costText != null)
        {
            costText.text = $"${itemData.Cost}";
            costText.gameObject.SetActive(showCost);
        }

        if (countText != null)
        {
            int count = inventoryManager != null ? inventoryManager.GetCount(itemData) : 0;
            countText.text = showMaxCount ? $"{count}/{itemData.MaxInventory}" : count.ToString();
            countText.gameObject.SetActive(showCount);
        }

        if (iconImage != null)
        {
            iconImage.sprite = itemData.Icon;
            iconImage.enabled = itemData.Icon != null;
        }

        OnSelectionChanged(selectionManager != null ? selectionManager.SelectedItem : null);
    }

    public void SetBackgroundSprites(Sprite normalSprite, Sprite selectedSprite)
    {
        normalBackgroundSprite = normalSprite;
        selectedBackgroundSprite = selectedSprite != null ? selectedSprite : normalSprite;
        OnSelectionChanged(selectionManager != null ? selectionManager.SelectedItem : null);
    }

    private void OnSelectionChanged(ShelterItemData selectedItem)
    {
        if (background == null)
            return;

        bool isSelected = selectedItem == itemData;
        background.sprite = isSelected && selectedBackgroundSprite != null
            ? selectedBackgroundSprite
            : normalBackgroundSprite;
        background.type = Image.Type.Simple;
        background.color = isSelected ? selectedColor : normalColor;
    }

    private void EnsureVisualChildren()
    {
        background = GetComponent<Image>();
        if (background != null)
        {
            background.sprite = normalBackgroundSprite;
            background.type = Image.Type.Simple;
            background.color = normalColor;
            background.raycastTarget = true;
        }

        iconImage = FindChildComponent<Image>("Icon") != null ? FindChildComponent<Image>("Icon") : iconImage;
        labelText = FindChildComponent<TextMeshProUGUI>("Text (TMP)") != null ? FindChildComponent<TextMeshProUGUI>("Text (TMP)") : labelText;
        labelText = labelText != null ? labelText : FindChildComponent<TextMeshProUGUI>("Label");
        costText = FindChildComponent<TextMeshProUGUI>("Cost") != null ? FindChildComponent<TextMeshProUGUI>("Cost") : costText;
        countText = FindChildComponent<TextMeshProUGUI>("Count") != null ? FindChildComponent<TextMeshProUGUI>("Count") : countText;

        iconImage = iconImage != null ? iconImage : CreateImage("Icon", Vector2.zero, new Vector2(46f, 46f));
        labelText = labelText != null
            ? labelText
            : CreateText("Label", new Vector2(122f, 0f), new Vector2(240f, 58f), 34, TextAlignmentOptions.MidlineLeft);
        costText = costText != null ? costText : CreateText("Cost", Vector2.zero, new Vector2(52f, 16f), 12, TextAlignmentOptions.Midline);
        countText = countText != null ? countText : CreateText("Count", Vector2.zero, new Vector2(36f, 16f), 12, TextAlignmentOptions.Midline);

        RemoveDuplicateChildren("Icon", iconImage.gameObject);
        RemoveDuplicateChildren("Cost", costText.gameObject);
        RemoveDuplicateChildren("Count", countText.gameObject);

        if (costText != null)
            costText.gameObject.SetActive(showCost);
        if (countText != null)
            countText.gameObject.SetActive(showCount);
    }

    private T FindChildComponent<T>(string childName) where T : Component
    {
        foreach (T component in GetComponentsInChildren<T>(true))
        {
            if (component.transform.parent == transform && component.name == childName)
                return component;
        }

        return null;
    }

    private void RemoveDuplicateChildren(string childName, GameObject keep)
    {
        for (int index = transform.childCount - 1; index >= 0; index--)
        {
            Transform child = transform.GetChild(index);
            if (child.gameObject == keep || child.name != childName)
                continue;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GameObject duplicate = child.gameObject;
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (duplicate != null)
                        DestroyImmediate(duplicate);
                };
            }
            else
#endif
                Destroy(child.gameObject);
        }
    }

    private Image CreateImage(string childName, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject child = new GameObject(childName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        child.transform.SetParent(transform, false);

        RectTransform rect = (RectTransform)child.transform;
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        Image image = child.GetComponent<Image>();
        image.preserveAspect = true;
        image.raycastTarget = false;
        return image;
    }

    private TextMeshProUGUI CreateText(string childName, Vector2 anchoredPosition, Vector2 sizeDelta, float fontSize, TextAlignmentOptions alignment)
    {
        GameObject child = new GameObject(childName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        child.transform.SetParent(transform, false);

        RectTransform rect = (RectTransform)child.transform;
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
        return ConfigureText(text, anchoredPosition, sizeDelta, fontSize, alignment);
    }

    private TextMeshProUGUI ConfigureText(TextMeshProUGUI text, Vector2 anchoredPosition, Vector2 sizeDelta, float fontSize, TextAlignmentOptions alignment)
    {
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        text.raycastTarget = false;
        text.fontSize = fontSize;
        text.enableAutoSizing = true;
        text.fontSizeMin = 14f;
        text.fontSizeMax = fontSize;
        text.alignment = alignment;
        text.color = new Color(1f, 0.88f, 0.54f, 1f);
        return text;
    }
}
