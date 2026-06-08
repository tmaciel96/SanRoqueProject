using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RescueItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI expirationText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;

    private RescueRequest currentRequest;
    private RescueManager manager;

    public void Setup(RescueRequest request, RescueManager rescueManager)
    {
        currentRequest = request;
        manager = rescueManager;

        nameText.text = request.animalData.animalName;

        expirationText.text = request.daysRemaining == 1
            ? "¡Último día!"
            : $"Expira en {request.daysRemaining} Días";

        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(ClickAccept);

        rejectButton.onClick.RemoveAllListeners();
        rejectButton.onClick.AddListener(ClickReject);
    }

    private void ClickAccept()
    {
        if (!manager.AcceptRescue(currentRequest))
        {
            Debug.LogWarning("No hay capacidad para aceptar este rescate.");
            return;
        }
        // Si acepta, RescuePanelUI.RefreshList lo maneja via OnRequestsUpdated
    }

    private void ClickReject()
    {
        manager.RejectRescue(currentRequest);
        // Idem
    }
}