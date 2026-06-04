using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Serializable]
    private class ItemStock
    {
        public ShelterItemData itemData;
        [Min(0)] public int count;
    }

    public static InventoryManager Instance { get; private set; }

    [Header("Economy")]
    [SerializeField] private int startingMoney = 1250;

    [Header("Starting Inventory")]
    [SerializeField] private List<ItemStock> startingItems = new List<ItemStock>();

    private readonly Dictionary<ShelterItemData, int> itemCounts = new Dictionary<ShelterItemData, int>();

    public event Action Changed;

    public int Money { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Money = Mathf.Max(0, startingMoney);

        foreach (ItemStock stock in startingItems)
        {
            if (stock.itemData == null)
                continue;

            itemCounts[stock.itemData] = Mathf.Clamp(stock.count, 0, stock.itemData.MaxInventory);
        }
    }

    public void RegisterItem(ShelterItemData itemData)
    {
        if (itemData == null || itemCounts.ContainsKey(itemData))
            return;

        itemCounts[itemData] = 0;
        Changed?.Invoke();
    }

    public int GetCount(ShelterItemData itemData)
    {
        if (itemData == null)
            return 0;

        return itemCounts.TryGetValue(itemData, out int count) ? count : 0;
    }

    public bool HasItem(ShelterItemData itemData)
    {
        return GetCount(itemData) > 0;
    }

    public bool TryBuyItem(ShelterItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("Inventario: no se puede comprar un item nulo.");
            return false;
        }

        RegisterItem(itemData);

        int currentCount = GetCount(itemData);
        if (currentCount >= itemData.MaxInventory)
        {
            Debug.Log($"Inventario: {itemData.DisplayName} ya esta al maximo ({currentCount}/{itemData.MaxInventory}).");
            return false;
        }

        if (Money < itemData.Cost)
        {
            Debug.Log($"Inventario: dinero insuficiente para comprar {itemData.DisplayName}. Costo: ${itemData.Cost}, disponible: ${Money}.");
            return false;
        }

        Money -= itemData.Cost;
        itemCounts[itemData] = currentCount + 1;
        Debug.Log($"Inventario: compraste {itemData.DisplayName}. Stock: {itemCounts[itemData]}/{itemData.MaxInventory}. Dinero: ${Money}.");
        Changed?.Invoke();
        return true;
    }

    public bool TryConsumeItem(ShelterItemData itemData)
    {
        if (!HasItem(itemData))
        {
            Debug.Log($"Inventario: no quedan unidades de {itemData?.DisplayName ?? "este item"}.");
            return false;
        }

        itemCounts[itemData]--;
        Debug.Log($"Inventario: usaste {itemData.DisplayName}. Stock: {itemCounts[itemData]}/{itemData.MaxInventory}.");
        Changed?.Invoke();
        return true;
    }
}
