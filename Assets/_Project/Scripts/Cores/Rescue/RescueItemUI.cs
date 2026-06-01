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

        nameText.text = $"{request.animalData.animalName}";
        expirationText.text = $"Expira en {request.expirationDay} Día{(request.expirationDay > 1 ? "s" : "")}";

        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(ClickAccept);

        rejectButton.onClick.RemoveAllListeners();
        rejectButton.onClick.AddListener(ClickReject);
    }

    private void ClickAccept()
    {
        if (manager.AcceptRescue(currentRequest))
        {
            Destroy(gameObject);
        }
    }

    private void ClickReject()
    {
        manager.RejectRescue(currentRequest);
        Destroy(gameObject);
    }
}