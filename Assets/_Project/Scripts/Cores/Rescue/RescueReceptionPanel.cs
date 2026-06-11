using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel de recepción al completarse un rescate. Hereda de BasePanel.
/// Se abre automáticamente via RescueManager.OnRescueCompleted.
/// pausesTime = true (default de BasePanel) — congela el juego hasta que el player confirma.
///
/// Hierarchy sugerida:
/// RescueReceptionPanel (BasePanel, pausesTime = true)
///   Background  (Image semitransparente)
///   Card
///     Title       "¡Animal rescatado!"
///     AnimalIcon  (Image)
///     AnimalName  (TextMeshProUGUI)
///     SpeciesText (TextMeshProUGUI)
///     RewardRow
///       RewardText (TextMeshProUGUI)  "+$100"
///     FlavourText (TextMeshProUGUI)
///     ConfirmButton "¡Genial!"
/// </summary>
public class RescueReceptionPanel : BasePanel
{
    [Header("Visuales")]
    [SerializeField] private Image animalIcon;
    [SerializeField] private TextMeshProUGUI animalNameText;
    [SerializeField] private TextMeshProUGUI speciesText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI flavourText;

    [Header("Íconos por especie")]
    [SerializeField] private Sprite[] speciesIcons;

    [Header("Botones")]
    [SerializeField] private Button confirmButton;

    private readonly string[] flavourMessages =
    {
        "¡Encontró su nuevo hogar!",
        "¡Gracias a vos, tiene una segunda oportunidad!",
        "¡Está feliz y a salvo!",
        "¡El corral le da la bienvenida!",
        "¡Otro animal rescatado con amor!"
    };

    // ── Unity ─────────────────────────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake(); // Registra en UIManager

        confirmButton.onClick.AddListener(Close);
        RescueManager.OnRescueCompleted += HandleRescueCompleted;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        confirmButton.onClick.RemoveListener(Close);
        RescueManager.OnRescueCompleted -= HandleRescueCompleted;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    // ── Handler ───────────────────────────────────────────────────────────────

    private void HandleRescueCompleted(RescueRequest completedRequest)
    {
        Populate(completedRequest);

        // El animal llega al corral — registralo en el CapacityManager
        CapacityManager.Instance?.TryAddAnimal();

        Open(); // Usa base.Open() → activa GO + registra en UIManager (pausa el tiempo)
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void Populate(RescueRequest request)
    {
        var animal = request.animalData;

        if (animalIcon != null && speciesIcons != null)
        {
            int idx = (int)animal.species;
            if (idx >= 0 && idx < speciesIcons.Length)
                animalIcon.sprite = speciesIcons[idx];
        }

        animalNameText.text = animal.animalName;
        speciesText.text    = animal.species.ToString();
        rewardText.text     = $"+${request.rewardAmount}";
        flavourText.text    = flavourMessages[Random.Range(0, flavourMessages.Length)];
    }
}