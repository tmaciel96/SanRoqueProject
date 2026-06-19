using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [Header("Referencias")]
    [SerializeField] private AnimalPrefabCatalog animalPrefabCatalog;

    private const int MaxAnimalNameLength = 18;
    private static readonly Color NameCaretColor = new Color(0.18f, 0.1f, 0.03f, 1f);
    private static readonly Color NameSelectionColor = new Color(0.95f, 0.72f, 0.35f, 0f);
    private static readonly Color NameEditBlinkColor = new Color(0.95f, 0.72f, 0.35f, 1f);

    private int _row;
    private GameObject _spawnedAnimal;
    private Animal _currentAnimal;
    private AnimalData _currentAnimalData;
    private TMP_InputField _animalNameInput;
    private Coroutine _nameEditBlinkRoutine;
    private Color _animalNameDefaultColor;
    private Graphic _animalNamePlateGraphic;
    private Color _animalNamePlateDefaultColor;
    private bool _isEditingAnimalName;
    private bool _hasAnimalNameDefaultColor;
    private bool _hasAnimalNamePlateDefaultColor;
    private int _column;
    private PenState _currentState;
    private ShelterGridManager _gridManager;

    public PenState CurrentState => _currentState;

    private void Awake()
    {
        SetupAnimalNameInput();
    }

    private void OnDestroy()
    {
        StopAnimalNameEditFeedback();

        if (_animalNameInput != null)
        {
            _animalNameInput.onEndEdit.RemoveListener(ApplyAnimalNameEdit);
            _animalNameInput.onValueChanged.RemoveListener(UpdateAnimalNamePreview);
        }
    }

    // ── Init ──────────────────────────────────────────────────────────────

    public void Init(int row, int column, PenState initialState, ShelterGridManager manager)
    {
        _row = row;
        _column = column;
        _currentState = initialState;
        _gridManager = manager;

        SetupAnimalNameInput();
        RefreshInfoPanel();
        UpdateVisuals();
    }

    // ── Click ─────────────────────────────────────────────────────────────

    public void OnClick()
    {

        if (_currentState == PenState.Available)
        {
            TryBuy();
            return;
        }

        if (_currentState == PenState.Occupied && IsPointerOverAnimalName())
            BeginAnimalNameEdit();
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

    public bool PlaceAnimal(AnimalData animalData)
    {
        if (_currentState != PenState.Empty || animalContainer == null)
            return false;

        if (_spawnedAnimal != null)
            Destroy(_spawnedAnimal);

        GameObject animalPrefab = animalPrefabCatalog.GetPrefab(animalData.species);
        if (animalPrefab == null)
        {
            Debug.LogError($"RescueManager: no hay prefab para {animalData.species}.");
            return false;
        }

        _spawnedAnimal = Instantiate(animalPrefab, animalContainer.transform);

        Animal animal = _spawnedAnimal.GetComponent<Animal>();
        _currentAnimal = animal;
        _currentAnimalData = animalData;

        if (animal != null)
        {

            Guid newUuid = Guid.NewGuid();
            string animalId = newUuid.ToString();
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
        SetAnimalNameText(animalData.animalName);
        RefreshInfoPanel();
        UpdateVisuals();
        return true;
    }

    public void RemoveAnimal()
    {
        StopAnimalNameEditFeedback();

        if (_spawnedAnimal != null)
        {
            Destroy(_spawnedAnimal);
            _spawnedAnimal = null;
        }

        _currentAnimal = null;
        _currentAnimalData = null;
        _currentState = PenState.Empty;
        SetAnimalNameText("");
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

        SetAnimalNameInputEnabled(_currentState == PenState.Occupied);
    }

    // ── IInventoryItemTarget ──────────────────────────────────────────────

    private void SetupAnimalNameInput()
    {
        if (txtAnimalName == null)
            return;

        if (!_hasAnimalNameDefaultColor)
        {
            _animalNameDefaultColor = txtAnimalName.color;
            _hasAnimalNameDefaultColor = true;
        }

        if (_animalNamePlateGraphic == null && txtAnimalName.transform.parent != null)
            _animalNamePlateGraphic = txtAnimalName.transform.parent.GetComponent<Graphic>();

        if (_animalNamePlateGraphic != null && !_hasAnimalNamePlateDefaultColor)
        {
            _animalNamePlateDefaultColor = _animalNamePlateGraphic.color;
            _hasAnimalNamePlateDefaultColor = true;
        }

        txtAnimalName.raycastTarget = true;

        if (_animalNameInput == null)
            _animalNameInput = txtAnimalName.GetComponent<TMP_InputField>();

        if (_animalNameInput == null)
            _animalNameInput = txtAnimalName.gameObject.AddComponent<TMP_InputField>();

        PenAnimalNameClickForwarder clickForwarder = txtAnimalName.GetComponent<PenAnimalNameClickForwarder>();
        if (clickForwarder == null)
            clickForwarder = txtAnimalName.gameObject.AddComponent<PenAnimalNameClickForwarder>();

        clickForwarder.Initialize(this);

        _animalNameInput.textComponent = txtAnimalName;
        _animalNameInput.characterLimit = MaxAnimalNameLength;
        _animalNameInput.lineType = TMP_InputField.LineType.SingleLine;
        _animalNameInput.contentType = TMP_InputField.ContentType.Name;
        _animalNameInput.transition = UnityEngine.UI.Selectable.Transition.None;
        _animalNameInput.customCaretColor = true;
        _animalNameInput.caretColor = NameCaretColor;
        _animalNameInput.selectionColor = NameSelectionColor;
        _animalNameInput.caretWidth = 1;
        _animalNameInput.onEndEdit.RemoveListener(ApplyAnimalNameEdit);
        _animalNameInput.onValueChanged.RemoveListener(UpdateAnimalNamePreview);
        _animalNameInput.onEndEdit.AddListener(ApplyAnimalNameEdit);
        _animalNameInput.onValueChanged.AddListener(UpdateAnimalNamePreview);

        SetAnimalNameInputEnabled(_currentState == PenState.Occupied);
    }

    private void SetAnimalNameInputEnabled(bool enabled)
    {
        if (_animalNameInput == null)
            return;

        _animalNameInput.interactable = enabled;
        _animalNameInput.readOnly = !enabled;
    }

    internal void BeginAnimalNameEdit()
    {
        SetupAnimalNameInput();

        if (_animalNameInput == null || _currentState != PenState.Occupied)
            return;

        SetAnimalNameInputEnabled(true);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(_animalNameInput.gameObject);

        _animalNameInput.Select();
        _animalNameInput.ActivateInputField();
        _animalNameInput.MoveTextEnd(false);
        StartAnimalNameEditFeedback();
    }

    private void UpdateAnimalNamePreview(string editedName)
    {
        string cleanName = CleanAnimalName(editedName);

        if (_currentAnimal != null)
            _currentAnimal.AnimalName = cleanName;

        if (_currentAnimalData != null)
            _currentAnimalData.animalName = cleanName;
    }

    private bool IsPointerOverAnimalName()
    {
        if (txtAnimalName == null)
            return false;

        Canvas canvas = txtAnimalName.canvas;
        Camera eventCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;

        return RectTransformUtility.RectangleContainsScreenPoint(
            txtAnimalName.rectTransform,
            Input.mousePosition,
            eventCamera);
    }

    private void ApplyAnimalNameEdit(string editedName)
    {
        if (_currentState != PenState.Occupied)
            return;

        StopAnimalNameEditFeedback();

        string cleanName = CleanAnimalName(editedName);
        if (string.IsNullOrEmpty(cleanName))
            cleanName = CleanAnimalName(_currentAnimal?.AnimalName ?? _currentAnimalData?.animalName);

        if (string.IsNullOrEmpty(cleanName))
            cleanName = "Sin nombre";

        if (_currentAnimal != null)
            _currentAnimal.AnimalName = cleanName;

        if (_currentAnimalData != null)
            _currentAnimalData.animalName = cleanName;

        SetAnimalNameText(cleanName);
    }

    private void SetAnimalNameText(string animalName)
    {
        if (txtAnimalName == null)
            return;

        string cleanName = CleanAnimalName(animalName);

        if (_animalNameInput != null)
            _animalNameInput.SetTextWithoutNotify(cleanName);
        else
            txtAnimalName.text = cleanName;
    }

    private string CleanAnimalName(string animalName)
    {
        if (string.IsNullOrWhiteSpace(animalName))
            return "";

        string cleanName = animalName.Replace("\r", "").Replace("\n", "").Trim();
        return cleanName.Length <= MaxAnimalNameLength
            ? cleanName
            : cleanName.Substring(0, MaxAnimalNameLength);
    }

    internal void StartAnimalNameEditFeedback()
    {
        if (txtAnimalName == null)
            return;

        _isEditingAnimalName = true;

        if (_nameEditBlinkRoutine != null)
            return;

        _nameEditBlinkRoutine = StartCoroutine(BlinkAnimalNameWhileEditing());
    }

    internal void StopAnimalNameEditFeedback()
    {
        _isEditingAnimalName = false;

        if (_nameEditBlinkRoutine != null)
        {
            StopCoroutine(_nameEditBlinkRoutine);
            _nameEditBlinkRoutine = null;
        }

        if (txtAnimalName != null && _hasAnimalNameDefaultColor)
            txtAnimalName.color = _animalNameDefaultColor;

        if (_animalNamePlateGraphic != null && _hasAnimalNamePlateDefaultColor)
            _animalNamePlateGraphic.color = _animalNamePlateDefaultColor;
    }

    private IEnumerator BlinkAnimalNameWhileEditing()
    {
        bool highlighted = false;

        while (_isEditingAnimalName)
        {
            highlighted = !highlighted;
            txtAnimalName.color = highlighted ? NameEditBlinkColor : _animalNameDefaultColor;
            if (_animalNamePlateGraphic != null && _hasAnimalNamePlateDefaultColor)
                _animalNamePlateGraphic.color = highlighted
                    ? Color.Lerp(_animalNamePlateDefaultColor, NameEditBlinkColor, 0.35f)
                    : _animalNamePlateDefaultColor;

            yield return new WaitForSecondsRealtime(0.35f);
        }

        _nameEditBlinkRoutine = null;

        if (txtAnimalName != null && _hasAnimalNameDefaultColor)
            txtAnimalName.color = _animalNameDefaultColor;

        if (_animalNamePlateGraphic != null && _hasAnimalNamePlateDefaultColor)
            _animalNamePlateGraphic.color = _animalNamePlateDefaultColor;
    }

    public bool CanReceiveItem(ShelterItemData itemData) => false;
    public bool ApplyItem(ShelterItemData itemData) => false;
    public void SetItemPreview(bool active, ShelterItemData itemData) { }
}

internal sealed class PenAnimalNameClickForwarder : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    private Pen pen;

    public void Initialize(Pen owner)
    {
        pen = owner;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            pen?.BeginAnimalNameEdit();
    }

    public void OnSelect(BaseEventData eventData)
    {
        pen?.StartAnimalNameEditFeedback();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        pen?.StopAnimalNameEditFeedback();
    }
}
