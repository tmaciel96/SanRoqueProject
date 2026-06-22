using UnityEngine;

public class CareSource : MonoBehaviour
{
    [SerializeField] private GameObject itemPrfb;
    [SerializeField] private CareType careType;           // asignar en Inspector
    [SerializeField] private InventoryItemType inventoryType; // asignar en Inspector
    [SerializeField] private bool consumesInventory = true;   // Water = false

    private void OnMouseDown()
    {
        if (consumesInventory && !InventoryManager.Instance.HasStock(inventoryType))
        {
            Debug.Log($"[CareSource] Sin stock de {inventoryType}");
            // TODO: podés agregar feedback visual aquí (shake, sonido, etc.)
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        GameObject portion = Instantiate(itemPrfb, mousePosition, Quaternion.identity);
        DraggableItem draggableItem = portion.GetComponent<DraggableItem>();
        if (draggableItem != null) draggableItem.BeginDrag();
    }
}