using UnityEngine;

/// <summary>
/// Maneja el stock de insumos comprables: comida, medicina y juguetes.
/// Separado del WaterManager intencionalmente.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Stock inicial")]
    [SerializeField] private int initialFood = 0;
    [SerializeField] private int initialMedicine = 0;
    [SerializeField] private int initialToys = 0;

    // ── Stock actual ──────────────────────────────────────────────────────

    public int Food { get; private set; }
    public int Medicine { get; private set; }
    public int Toys { get; private set; }

    // ── Eventos ───────────────────────────────────────────────────────────

    public static event System.Action OnInventoryChanged;

    // ── Ciclo de vida ─────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        Food = initialFood;
        Medicine = initialMedicine;
        Toys = initialToys;
    }

    // ── API genérica (usada por ShopItemPanelUI) ──────────────────────────

    public int GetStock(InventoryItemType type)
    {
        return type switch
        {
            InventoryItemType.Food => Food,
            InventoryItemType.Medicine => Medicine,
            InventoryItemType.Toy => Toys,
            _ => 0
        };
    }

    public void AddItem(InventoryItemType type, int amount)
    {
        if (amount <= 0) return;

        switch (type)
        {
            case InventoryItemType.Food: Food += amount; break;
            case InventoryItemType.Medicine: Medicine += amount; break;
            case InventoryItemType.Toy: Toys += amount; break;
        }

        OnInventoryChanged?.Invoke();
        Debug.Log($"InventoryManager: +{amount} {type}. Nuevo stock: {GetStock(type)}");
    }

    public void ConsumeItem(InventoryItemType type, int amount)
    {
        if (amount <= 0) return;

        switch (type)
        {
            case InventoryItemType.Food: Food = Mathf.Max(0, Food - amount); break;
            case InventoryItemType.Medicine: Medicine = Mathf.Max(0, Medicine - amount); break;
            case InventoryItemType.Toy: Toys = Mathf.Max(0, Toys - amount); break;
        }

        OnInventoryChanged?.Invoke();
    }

    public bool HasStock(InventoryItemType type, int amount = 1)
    {
        return GetStock(type) >= amount;
    }
}