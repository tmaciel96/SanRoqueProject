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

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        MoneyManager.Instance.OnMoneyChanged += HandleMoneyChanged;
        RefreshInteractable();
    }

    private void OnDisable()
    {
        MoneyManager.Instance.OnMoneyChanged -= HandleMoneyChanged;
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

    private void RefreshInteractable()
    {
        if (itemData == null) return;
        _button.interactable = MoneyManager.Instance.CanAfford(itemData.pricePerUnit);
    }
}