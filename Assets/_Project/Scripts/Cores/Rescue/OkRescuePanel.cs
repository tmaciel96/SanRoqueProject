using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel que aparece al completarse un rescate.
/// Muestra un mensaje y un botón "Ubicar" para asignar el animal al grid.
///
/// Hierarchy:
/// OkRescuePanel (BasePanel)
///   Mensaje     (TextMeshProUGUI)
///   BtnUbicar   (Button)
///     Text (TMP)
/// </summary>
public class OkRescuePanel : BasePanel
{
    [Header("Visuales")]
    [SerializeField] private TextMeshProUGUI mensajeText;

    [Header("Botones")]
    [SerializeField] private Button btnUbicar;

    private RescueRequest completedRequest;

    // ── Unity ─────────────────────────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake();
        btnUbicar.onClick.AddListener(HandleUbicar);
        RescueManager.OnRescueCompleted += HandleRescueCompleted;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        btnUbicar.onClick.RemoveListener(HandleUbicar);
        RescueManager.OnRescueCompleted -= HandleRescueCompleted;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    // ── Handlers ─────────────────────────────────────────────────────────────

    private void HandleRescueCompleted(RescueRequest request)
    {
        Debug.Log($"[OkRescuePanel] Rescue completed: {request?.animalData?.animalName}");
        completedRequest = request;
        Populate(request);
        Open();
    }

    private void HandleUbicar()
    {
        // TODO: lógica de asignación al grid
        Close();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void Populate(RescueRequest request)
    {
        if (mensajeText != null)
            mensajeText.text = $"¡{request.animalData.animalName} llegó al corral!";
    }
}