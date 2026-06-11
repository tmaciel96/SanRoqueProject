using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Indicador persistente en la HUD que muestra el rescate en progreso.
/// Aparece al iniciarse un rescate y desaparece al completarse.
///
/// Hierarchy sugerida (en la HUD principal):
/// RescueInProgressIndicator
///   Icon         (Image) — ícono del animal en viaje
///   AnimalName   (TextMeshProUGUI)
///   HoursLeft    (TextMeshProUGUI) — "Llega en 2 horas"
///   ProgressBar  (Slider, opcional)
/// </summary>
public class RescueInProgressIndicator : MonoBehaviour
{
    [Header("Visuales")]
    [SerializeField] private Image animalIcon;
    [SerializeField] private TextMeshProUGUI animalNameText;
    [SerializeField] private TextMeshProUGUI hoursLeftText;
    [SerializeField] private Slider progressBar;

    [Header("Íconos por especie")]
    [SerializeField] private Sprite[] speciesIcons;

    [Header("Referencia")]
    [SerializeField] private RescueManager rescueManager;

    private RescueRequest trackedRequest;

    // ── Unity ─────────────────────────────────────────────────────────────────

    private void Awake()
    {
        RescueManager.OnRescueStarted   += HandleRescueStarted;
        RescueManager.OnRescueCompleted += HandleRescueCompleted;
        DayManager.OnHourChanged        += HandleHourChanged;
    }

    private void OnDestroy()
    {
        RescueManager.OnRescueStarted   -= HandleRescueStarted;
        RescueManager.OnRescueCompleted -= HandleRescueCompleted;
        DayManager.OnHourChanged        -= HandleHourChanged;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    // ── Handlers ─────────────────────────────────────────────────────────────

    private void HandleRescueStarted(RescueRequest request)
    {
        trackedRequest = request;
        gameObject.SetActive(true);
        Refresh();
    }

    private void HandleRescueCompleted(RescueRequest request)
    {
        trackedRequest = null;
        gameObject.SetActive(false);
    }

    private void HandleHourChanged(int hour)
    {
        // El tick ya fue procesado por RescueManager, solo refrescamos la UI
        if (trackedRequest == null) return;
        Refresh();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void Refresh()
    {
        if (trackedRequest == null) return;

        // Ícono
        if (animalIcon != null && speciesIcons != null)
        {
            int idx = (int)trackedRequest.animalData.species;
            if (idx >= 0 && idx < speciesIcons.Length)
                animalIcon.sprite = speciesIcons[idx];
        }

        // Nombre
        animalNameText.text = trackedRequest.animalData.animalName;

        // Horas restantes
        int hoursLeft = trackedRequest.hoursRemainingInRescue;
        hoursLeftText.text = hoursLeft <= 1
            ? "¡Llega en menos de 1 hora!"
            : $"Llega en {hoursLeft} horas";

        // Barra de progreso (0 = inicio, 1 = completo)
        if (progressBar != null)
        {
            float total   = trackedRequest.rescueDurationHours;
            float elapsed = total - trackedRequest.hoursRemainingInRescue;
            progressBar.value = total > 0 ? elapsed / total : 0f;
        }
    }
}