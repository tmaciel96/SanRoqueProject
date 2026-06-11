using UnityEngine;
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
    [SerializeField] private TextMeshProUGUI txtAnimalName;

    private int _row;
    private GameObject _spawnedAnimal;
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

        if (_currentState == PenState.Available)
            TryBuy();
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

    public bool PlaceAnimal(GameObject animalPrefab, AnimalData animalData, int animalId)
    {
        if (_currentState != PenState.Empty || animalPrefab == null || animalContainer == null)
            return false;

        if (_spawnedAnimal != null)
            Destroy(_spawnedAnimal);

        _spawnedAnimal = Instantiate(animalPrefab, animalContainer.transform);

        Animal animal = _spawnedAnimal.GetComponent<Animal>();
        if (animal != null)
        {
            animal.Initialize(
                animalId,
                animalData.animalName,
                animalData.species,
                animalData.hunger,
                animalData.thirst,
                animalData.affection,
                animalData.health,
                animalData.variantIndex);
        }

        _currentState = PenState.Occupied;
        txtAnimalName.text = animalData.animalName;
        RefreshInfoPanel();
        UpdateVisuals();
        return true;
    }

    public void RemoveAnimal()
    {
        if (_spawnedAnimal != null)
        {
            Destroy(_spawnedAnimal);
            _spawnedAnimal = null;
        }

        _currentState = PenState.Empty;
        txtAnimalName.text = "";
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