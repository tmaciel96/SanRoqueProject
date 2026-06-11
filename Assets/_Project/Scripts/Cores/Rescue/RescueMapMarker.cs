using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Marker visual en el mapa — solo un pin genérico clickeable.
/// Sin nombre ni timer: toda la info va al panel lateral al hacer click.
///
/// Prefab mínimo:
///   RescueMapMarker (Button + este script)
///     PinImage  (Image) — el ícono de pin, siempre igual para todos
///     (opcional) PulseAnimator para un idle suave
/// </summary>
[RequireComponent(typeof(Button))]
public class RescueMapMarker : MonoBehaviour
{
    [Header("Visuales")]
    [SerializeField] private Image pinImage;
    [SerializeField] private Sprite defaultPinSprite;
    [SerializeField] private Sprite urgentPinSprite;   // Opcional: distinto color si expira mañana

    private RescueRequest request;
    private Button button;

    public static event System.Action<RescueRequest> OnMarkerClicked;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(HandleClick);
    }

    public void Setup(RescueRequest rescueRequest)
    {
        request = rescueRequest;
        Refresh();
    }

    public void Refresh()
    {
        if (request == null || pinImage == null) return;

        // Si queda 1 día, mostrá el pin urgente (opcional)
        bool isUrgent = request.hoursUntilExpiry <= 1;
        if (urgentPinSprite != null)
            pinImage.sprite = isUrgent ? urgentPinSprite : defaultPinSprite;
        else if (defaultPinSprite != null)
            pinImage.sprite = defaultPinSprite;
    }

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
    }

    private void HandleClick()
    {
            Debug.Log($"[RescueMapMarker] Click en {request?.animalData?.animalName}");

        OnMarkerClicked?.Invoke(request);
    }
}