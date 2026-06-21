using UnityEngine;
using TMPro;

/// <summary>
/// Refresca los textos de la sidebar de recursos (food/medicine/toys)
/// cada vez que InventoryManager.OnInventoryChanged se dispara.
/// Asignar los TextMeshProUGUI correspondientes en el Inspector.
/// </summary>
public class ResourceSidebarUI : MonoBehaviour
{
    [Header("Textos de cantidad")]
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI medicineText;
    [SerializeField] private TextMeshProUGUI toysText;

    // TODO: cuando exista WaterManager, agregar waterText y suscribirse
    // a su evento de cambio (probablemente algo como OnWaterChanged).
    // [SerializeField] private TextMeshProUGUI waterText;

    private void OnEnable()
    {
        InventoryManager.OnInventoryChanged += RefreshAll;
        RefreshAll();
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= RefreshAll;
    }

    private void RefreshAll()
    {
        if (InventoryManager.Instance == null) return;

        foodText.text = InventoryManager.Instance.Food.ToString();
        medicineText.text = InventoryManager.Instance.Medicine.ToString();
        toysText.text = InventoryManager.Instance.Toys.ToString();
    }
}