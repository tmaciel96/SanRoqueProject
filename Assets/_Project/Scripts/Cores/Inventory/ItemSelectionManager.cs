using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSelectionManager : MonoBehaviour
{
    public static ItemSelectionManager Instance { get; private set; }

    private Camera mainCamera;
    private IInventoryItemTarget previewTarget;

    public event Action<ShelterItemData> SelectionChanged;

    public ShelterItemData SelectedItem { get; private set; }
    public bool HasSelection => SelectedItem != null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdatePreviewTarget();
        ReportInvalidWorldClick();
    }

    public void SelectItem(ShelterItemData itemData)
    {
        if (itemData == null)
        {
            ClearSelection();
            return;
        }

        if (InventoryManager.Instance == null || !InventoryManager.Instance.HasItem(itemData))
        {
            Debug.Log($"Seleccion: no hay unidades disponibles de {itemData.DisplayName}.");
            return;
        }

        SelectedItem = itemData;
        Debug.Log($"Seleccion: {itemData.DisplayName} seleccionado. Hace click en un objetivo valido para aplicarlo.");
        SelectionChanged?.Invoke(SelectedItem);
    }

    public void ClearSelection()
    {
        if (previewTarget != null)
            previewTarget.SetItemPreview(false, SelectedItem);

        previewTarget = null;
        SelectedItem = null;
        SelectionChanged?.Invoke(null);
    }

    public bool TryApplyToTarget(IInventoryItemTarget target)
    {
        if (SelectedItem == null || target == null)
            return false;

        if (!target.CanReceiveItem(SelectedItem))
        {
            Debug.Log($"Seleccion: {SelectedItem.DisplayName} no se puede aplicar sobre este objetivo.");
            return false;
        }

        if (InventoryManager.Instance == null || !InventoryManager.Instance.HasItem(SelectedItem))
        {
            Debug.Log($"Seleccion: no quedan unidades de {SelectedItem.DisplayName}.");
            ClearSelection();
            return false;
        }

        ShelterItemData appliedItem = SelectedItem;
        if (!target.ApplyItem(appliedItem))
            return false;

        InventoryManager.Instance.TryConsumeItem(appliedItem);

        if (!InventoryManager.Instance.HasItem(appliedItem))
            ClearSelection();
        else
            SelectionChanged?.Invoke(SelectedItem);

        return true;
    }

    private void UpdatePreviewTarget()
    {
        IInventoryItemTarget newTarget = null;

        if (SelectedItem != null && !IsPointerOverUi())
        {
            Collider2D hoveredCollider = GetColliderUnderMouse();
            if (hoveredCollider != null)
                newTarget = hoveredCollider.GetComponentInParent<IInventoryItemTarget>();

            if (newTarget != null && !newTarget.CanReceiveItem(SelectedItem))
                newTarget = null;
        }

        if (previewTarget == newTarget)
            return;

        if (previewTarget != null)
            previewTarget.SetItemPreview(false, SelectedItem);

        previewTarget = newTarget;

        if (previewTarget != null)
            previewTarget.SetItemPreview(true, SelectedItem);
    }

    private void ReportInvalidWorldClick()
    {
        if (SelectedItem == null || !Input.GetMouseButtonDown(0) || IsPointerOverUi())
            return;

        Collider2D clickedCollider = GetColliderUnderMouse();
        IInventoryItemTarget target = clickedCollider != null
            ? clickedCollider.GetComponentInParent<IInventoryItemTarget>()
            : null;

        if (target == null || !target.CanReceiveItem(SelectedItem))
            Debug.Log($"Seleccion: {SelectedItem.DisplayName} sigue seleccionado. Ese no es un objetivo valido.");
    }

    private Collider2D GetColliderUnderMouse()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return null;

        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return Physics2D.OverlapPoint(worldPoint);
    }

    private static bool IsPointerOverUi()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
