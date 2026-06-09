using UnityEngine;

/// <summary>
/// Datos de configuración de un ítem comprable en la tienda.
/// Crear un asset por tipo: ComidaItem, MedicinaItem, JugueteItem.
/// </summary>
[CreateAssetMenu(fileName = "ShopItemData", menuName = "SanRoque/Shop Item Data")]
public class ShopItemData : ScriptableObject
{
    [Header("Identidad")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Economía")]
    public int pricePerUnit = 10;

    [Header("Inventario")]
    /// <summary>
    /// Tipo de inventario que se incrementa al comprar.
    /// Debe coincidir con el campo que maneja InventoryManager.
    /// </summary>
    public InventoryItemType inventoryType;
}

/// <summary>
/// Enum que identifica qué contador del InventoryManager se modifica.
/// Agregar entradas si se suman nuevos insumos.
/// </summary>
public enum InventoryItemType
{
    Food,
    Medicine,
    Toy
}