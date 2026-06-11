using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Sub-panel de detalle dentro de RescuePanel (columna derecha).
/// MonoBehaviour simple — no hereda de BasePanel porque no es un panel independiente.
/// Se llena al clickear un marker en el mapa.
///
/// Hierarchy:
/// DetailPanel (este script)
///   EmptyState      — "Seleccioná un animal en el mapa"
///   FilledState
///     AnimalIcon    (Image)
///     AnimalNombre  (TextMeshProUGUI)
///     RowCost
///       ValueCost   (TextMeshProUGUI)  "2 horas"
///     RowReward
///       ValueReward (TextMeshProUGUI)  "$100"
///     ExpireDate    (TextMeshProUGUI)  "Expira en 2 horas"
///     Warnings
///       NoSpaceWarning
///       ActiveRescueWarning
///     BtnRescate    (Button)
///     BtnRechazar   (Button)
/// </summary>
public class RescueDetailPanel : MonoBehaviour
{
    [Header("Estados")]
    [SerializeField] private GameObject emptyState;
    [SerializeField] private GameObject filledState;

    [Header("Visuales")]
    [SerializeField] private Image animalIcon;
    [SerializeField] private TextMeshProUGUI animalNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI expiryText;

    [Header("Íconos por especie")]
    [SerializeField] private Sprite[] speciesIcons;

    [Header("Warnings")]
    [SerializeField] private GameObject noSpaceWarning;
    [SerializeField] private GameObject activeRescueWarning;

    [Header("Botones")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;

    [Header("Referencias")]
    [SerializeField] private RescueManager rescueManager;

    private RescueRequest currentRequest;

    // ── Unity ─────────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        acceptButton.onClick.AddListener(HandleAccept);
        rejectButton.onClick.AddListener(HandleReject);

        RescueMapMarker.OnMarkerClicked += LoadRequest;
        RescueManager.OnRescueStarted   += OnRescueStateChanged;
        RescueManager.OnRescueCompleted += OnRescueStateChanged;
    }

    private void OnDisable()
    {
        acceptButton.onClick.RemoveListener(HandleAccept);
        rejectButton.onClick.RemoveListener(HandleReject);

        RescueMapMarker.OnMarkerClicked -= LoadRequest;
        RescueManager.OnRescueStarted   -= OnRescueStateChanged;
        RescueManager.OnRescueCompleted -= OnRescueStateChanged;
    }

    private void Start()
    {
        ShowEmpty();
    }

    // ── API ───────────────────────────────────────────────────────────────────

    public void LoadRequest(RescueRequest request)
    {
        currentRequest = request;
        Populate();
        RefreshWarnings();
        emptyState.SetActive(false);
        filledState.SetActive(true);
    }

    public void ShowEmpty()
    {
        currentRequest = null;
        emptyState.SetActive(true);
        filledState.SetActive(false);
    }

    // ── Handlers ─────────────────────────────────────────────────────────────

    private void HandleAccept()
    {
        if (currentRequest == null) return;

        bool success = rescueManager.TryAcceptRescue(currentRequest);
        if (success)
            ShowEmpty();
        else
            RefreshWarnings();
    }

    private void HandleReject()
    {
        if (currentRequest == null) return;
        rescueManager.RejectRescue(currentRequest);
        ShowEmpty();
    }

    private void OnRescueStateChanged(RescueRequest _) => RefreshWarnings();

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void Populate()
    {
        if (currentRequest == null) return;

        if (animalIcon != null && speciesIcons != null)
        {
            int idx = (int)currentRequest.animalData.species;
            if (idx >= 0 && idx < speciesIcons.Length)
                animalIcon.sprite = speciesIcons[idx];
        }

        animalNameText.text = currentRequest.animalData.animalName;

        int hours = currentRequest.rescueDurationHours;
        costText.text = hours == 1 ? "1 hora" : $"{hours} horas";

        rewardText.text = $"${currentRequest.rewardAmount}";

        int expiry = currentRequest.hoursUntilExpiry;
        expiryText.text = expiry == 1 ? "¡Última hora!" : $"Expira en {expiry} horas";
    }

    private void RefreshWarnings()
    {
        if (currentRequest == null) return;

        bool hasActiveRescue = rescueManager.HasActiveRescue;
        var  cm       = CapacityManager.Instance;
        bool hasSpace = cm != null && cm.HasShelter && cm.CurrentAnimals < cm.MaxCapacity;

        noSpaceWarning?.SetActive(!hasSpace && !hasActiveRescue);
        activeRescueWarning?.SetActive(hasActiveRescue);
        acceptButton.interactable = !hasActiveRescue && hasSpace;

        // Texto con tiempo restante del rescate activo
        if (hasActiveRescue && activeRescueWarning != null)
        {
            var text = activeRescueWarning.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                int hours = rescueManager.ActiveRescue.hoursRemainingInRescue;
                text.text = hours <= 1
                    ? "Hay un rescate en curso. ¡Llega en menos de 1 hora!"
                    : $"Hay un rescate en curso. Llega en {hours} horas.";
            }
        }
    }
}