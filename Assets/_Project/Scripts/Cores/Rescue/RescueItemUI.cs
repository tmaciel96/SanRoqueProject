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
    private RescuePanelUI panel;

    public void Setup(RescueRequest request, RescueManager rescueManager, RescuePanelUI rescuePanel)
    {
        currentRequest = request;
        manager = rescueManager;
        panel = rescuePanel;

        nameText.text = request.animalData.animalName;

        expirationText.text = request.daysRemaining == 1
            ? "¡Último día!"
            : $"Expira en {request.daysRemaining} Días";

        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(ClickAccept);

        rejectButton.onClick.RemoveAllListeners();
        rejectButton.onClick.AddListener(ClickReject);

        RefreshAcceptButton();
    }

    public void RefreshAcceptButton()
    {
        if (acceptButton == null || CapacityManager.Instance == null)
            return;

        acceptButton.interactable = CapacityManager.Instance.HasAvailableSpace;
    }

    private void ClickAccept()
    {
        if (!manager.AcceptRescue(currentRequest))
        {
            Debug.LogWarning("No hay capacidad para aceptar este rescate.");
            RefreshAcceptButton();
            return;
        }

        panel?.RefreshAcceptButtons();
        Destroy(gameObject);
    }

    private void ClickReject()
    {
        manager.RejectRescue(currentRequest);
        Destroy(gameObject);
    }
}
