using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel genérico de compra de insumos.
/// Un solo panel en escena; se reconfigura con Open(ShopItemData).
/// Registrado una sola vez en UIManager como ShopItemPanelUI.
/// </summary>
public class ShopItemPanelUI : BasePanel
{
    [Header("Referencias UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI currentStockText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private TextMeshProUGUI totalCostText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private Button closeButton;

    // ── Estado interno ────────────────────────────────────────────────────

    private ShopItemData _currentItem;
    private int _quantity = 1;

    // ── Inicialización ────────────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake();

        increaseButton.onClick.AddListener(IncreaseQuantity);
        decreaseButton.onClick.AddListener(DecreaseQuantity);
        buyButton.onClick.AddListener(ConfirmPurchase);
        closeButton.onClick.AddListener(Close);
    }

    // ── API pública ───────────────────────────────────────────────────────

    /// <summary>
    /// Abre el panel configurado para el ítem dado.
    /// Llamar desde UIManager.ShowShopItemPanel(data).
    /// </summary>
    public void Open(ShopItemData itemData)
    {
        _currentItem = itemData;
        _quantity = 1;
        RefreshStaticInfo();
        RefreshDynamicInfo();
        base.Open();
    }

    // Sobrescribimos Open() sin parámetros para que no se use accidentalmente sin datos.
    public override void Open()
    {
        Debug.LogWarning("ShopItemPanelUI.Open() llamado sin datos. Usar Open(ShopItemData).");
    }

    // ── Lógica de cantidad ────────────────────────────────────────────────

    private void IncreaseQuantity()
    {
        int maxAffordable = MaxAffordable();
        if (_quantity < maxAffordable)
        {
            _quantity++;
            RefreshDynamicInfo();
        }
    }

    private void DecreaseQuantity()
    {
        if (_quantity > 1)
        {
            _quantity--;
            RefreshDynamicInfo();
        }
    }

    private int MaxAffordable()
    {
        if (_currentItem == null || _currentItem.pricePerUnit <= 0) return 0;
        return MoneyManager.Instance.CurrentMoney / _currentItem.pricePerUnit;
    }

    // ── Compra ────────────────────────────────────────────────────────────

    private void ConfirmPurchase()
    {
        if (_currentItem == null) return;

        int totalCost = _currentItem.pricePerUnit * _quantity;

        if (!MoneyManager.Instance.TrySpend(totalCost)) return;

        InventoryManager.Instance.AddItem(_currentItem.inventoryType, _quantity);
        TaskManager.Instance.ReportShopPurchase(_currentItem.inventoryType, _quantity);

        _quantity = 1;
        RefreshDynamicInfo();
    }

    // ── UI Refresh ────────────────────────────────────────────────────────

    /// <summary>Info que no cambia mientras el panel está abierto.</summary>
    private void RefreshStaticInfo()
    {
        if (_currentItem == null) return;

        itemIcon.sprite = _currentItem.icon;
        itemNameText.text = _currentItem.itemName;
        descriptionText.text = _currentItem.description;
        priceText.text = $"${_currentItem.pricePerUnit} / unidad";
    }

    /// <summary>Info que cambia con la cantidad o el dinero disponible.</summary>
    private void RefreshDynamicInfo()
    {
        if (_currentItem == null) return;

        int stock = InventoryManager.Instance.GetStock(_currentItem.inventoryType);
        int totalCost = _currentItem.pricePerUnit * _quantity;
        int maxAffordable = MaxAffordable();

        currentStockText.text = stock.ToString();
        quantityText.text = _quantity.ToString();
        totalCostText.text = $"${totalCost}";

        // Botón comprar: solo activo si puede pagar al menos 1
        buyButton.interactable = MoneyManager.Instance.CanAfford(totalCost);

        // Botones +/-
        increaseButton.interactable = _quantity < maxAffordable;
        decreaseButton.interactable = _quantity > 1;
    }
}
