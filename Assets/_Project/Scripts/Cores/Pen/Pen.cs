using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PenState { Locked, Available, Empty, Occupied }

public class Pen : MonoBehaviour, IInventoryItemTarget
{
    [Header("Visuals")]
    [SerializeField] private GameObject visualLocked;
    [SerializeField] private GameObject visualAvailable;
    [SerializeField] private GameObject visualEmpty;
    [SerializeField] private GameObject animalContainer;

    [Header("UI - Available")]
    [SerializeField] private TextMeshProUGUI txtPrecio;

    [Header("UI - Info")]
    [SerializeField] private TextMeshProUGUI txtNivel;
    [SerializeField] private TextMeshProUGUI txtCapacidad;

    [Header("UI - Animal")]
    [SerializeField] private Image imgAnimal;

    private int _row;
    private int _column;
    private PenState _currentState;
    private ShelterGridManager _gridManager;

    public PenState CurrentState => _currentState;

    // ── Init ──────────────────────────────────────────────────────────────

    public void Init(int row, int column, PenState initialState, ShelterGridManager manager)
    {
        _row = row;
        _column = column;
        _currentState = initialState;
        _gridManager = manager;

        RefreshInfoPanel();
        UpdateVisuals();
    }

    // ── Click ─────────────────────────────────────────────────────────────

    public void OnClick()
    {
        if (ItemSelectionManager.Instance != null && ItemSelectionManager.Instance.TryApplyToTarget(this))
            return;

        switch (_currentState)
        {
            case PenState.Available:
                TryBuy();
                break;
            case PenState.Empty:
                // TODO: abrir panel de asignación de animal
                Debug.Log($"Pen [{_row},{_column}]: vacío, listo para recibir un animal.");
                break;
            case PenState.Occupied:
                // TODO: abrir panel de interacción con el animal
                Debug.Log($"Pen [{_row},{_column}]: ocupado.");
                break;
            case PenState.Locked:
                Debug.Log($"Pen [{_row},{_column}]: bloqueado.");
                break;
        }
    }

    private void TryBuy()
    {
        bool success = CapacityManager.Instance.TryUnlockNextShelter(_gridManager, _row, _column);

        if (success)
        {
            _currentState = PenState.Empty;
            RefreshInfoPanel();
            UpdateVisuals();
        }
    }

    // ── Estado ────────────────────────────────────────────────────────────

    public void SetAvailable()
    {
        if (_currentState == PenState.Locked)
        {
            _currentState = PenState.Available;
            UpdateVisuals();
        }
    }

    public void AssignAnimal(AnimalData animal)
    {
        _currentState = PenState.Occupied;
        RefreshInfoPanel();
        UpdateVisuals();
        // TODO: asignar sprite del animal cuando esté disponible
        // imgAnimal.sprite = animal.sprite;
    }

    public void RemoveAnimal()
    {
        _currentState = PenState.Empty;
        RefreshInfoPanel();
        UpdateVisuals();
    }

    // ── UI ────────────────────────────────────────────────────────────────

    private void RefreshAvailablePanel()
    {
        if (txtPrecio == null) return;
        txtPrecio.text = CapacityManager.Instance.GetNextUnlockCostText();
    }

    private void RefreshInfoPanel()
    {
        if (txtNivel != null)
            txtNivel.text = "Nv.1";

        if (txtCapacidad != null)
            txtCapacidad.text = CapacityManager.Instance.GetTierCapacityText(_row, _column);
    }

    // ── Visuals ───────────────────────────────────────────────────────────

    private void UpdateVisuals()
    {
        visualLocked?.SetActive(false);
        visualAvailable?.SetActive(false);
        visualEmpty?.SetActive(false);
        animalContainer?.SetActive(false);

        switch (_currentState)
        {
            case PenState.Locked:
                visualLocked?.SetActive(true);
                break;
            case PenState.Available:
                visualAvailable?.SetActive(true);
                RefreshAvailablePanel();
                break;
            case PenState.Empty:
                visualEmpty?.SetActive(true);
                break;
            case PenState.Occupied:
                visualEmpty?.SetActive(true);
                animalContainer?.SetActive(true);
                break;
        }
    }

    // ── IInventoryItemTarget ──────────────────────────────────────────────

    public bool CanReceiveItem(ShelterItemData itemData) => false;
    public bool ApplyItem(ShelterItemData itemData) => false;
    public void SetItemPreview(bool active, ShelterItemData itemData) { }
}