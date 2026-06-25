using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Componente para cada botón "+" del sidebar de tienda.
/// Compra 1 unidad directamente al click (sin abrir ShopItemPanelUI).
/// Asignar el ShopItemData correspondiente en el Inspector.
/// </summary>
[RequireComponent(typeof(Button))]
public class ShopSidebarButton : MonoBehaviour
{
    [SerializeField] private ShopItemData itemData;

    private Button _button;
    private bool _isSubscribedToMoneyChanges;
    private MoneyManager _subscribedMoneyManager;

    private void Awake()
    {
        EnsureButton();
    }

    private void OnEnable()
    {
        EnsureButton();
        SubscribeToMoneyChanges();
        RefreshInteractable();
    }

    private void Start()
    {
        EnsureButton();
        SubscribeToMoneyChanges();
        RefreshInteractable();
    }

    private void OnDisable()
    {
        UnsubscribeFromMoneyChanges();
    }

    private void OnClick()
    {
        if (itemData == null)
        {
            Debug.LogWarning("ShopSidebarButton: itemData no asignado.");
            return;
        }

        int cost = itemData.pricePerUnit;

        // Descuenta dinero. Si no alcanza, TrySpend devuelve false (no debería
        // pasar si el botón está bien deshabilitado, pero se chequea igual).
        if (!MoneyManager.Instance.TrySpend(cost)) return;

        // Suma al inventario. Esto debería disparar OnInventoryChanged para
        // que la sidebar refresque las cantidades en pantalla.
        InventoryManager.Instance.AddItem(itemData.inventoryType, 1);

        // Reporta la compra para las tasks de Tienda.
        TaskManager.Instance.ReportShopPurchase(itemData.inventoryType, 1);
    }

    // ── UI ────────────────────────────────────────────────────────────────

    private void HandleMoneyChanged(int currentMoney)
    {
        RefreshInteractable();
    }

    private void SubscribeToMoneyChanges()
    {
        MoneyManager moneyManager = MoneyManager.Instance;
        if (_isSubscribedToMoneyChanges || moneyManager == null) return;

        moneyManager.OnMoneyChanged += HandleMoneyChanged;
        _subscribedMoneyManager = moneyManager;
        _isSubscribedToMoneyChanges = true;
    }

    private void UnsubscribeFromMoneyChanges()
    {
        if (!_isSubscribedToMoneyChanges || _subscribedMoneyManager == null) return;

        _subscribedMoneyManager.OnMoneyChanged -= HandleMoneyChanged;
        _subscribedMoneyManager = null;
        _isSubscribedToMoneyChanges = false;
    }

    private void RefreshInteractable()
    {
        if (_button == null || itemData == null || MoneyManager.Instance == null) return;
        _button.interactable = MoneyManager.Instance.CanAfford(itemData.pricePerUnit);
    }

    private void EnsureButton()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        if (_button == null) return;

        _button.onClick.RemoveListener(OnClick);
        _button.onClick.AddListener(OnClick);
    }
}
