using UnityEngine;
using System.Collections.Generic;

public class RescuePanelUI : BasePanel
{
    [Header("Referencias")]
    [SerializeField] private RescueManager rescueManager;
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject itemPrefab;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        RescueManager.OnRequestsUpdated += HandleDayRequestsUpdated;
        CapacityManager.OnCapacityChanged += RefreshAcceptButtons;
    }

    private void OnDisable()
    {
        RescueManager.OnRequestsUpdated -= HandleDayRequestsUpdated;
        CapacityManager.OnCapacityChanged -= RefreshAcceptButtons;
    }

    public override void Open()
    {
        base.Open();
        RefreshList();
        RefreshAcceptButtons();
    }

    private void HandleDayRequestsUpdated()
    {
        RefreshList();
        RefreshAcceptButtons();
    }

    private void RefreshList()
    {
        if (!gameObject.activeSelf) return;

        foreach (Transform child in listParent)
            Destroy(child.gameObject);

        List<RescueRequest> requests = rescueManager.GetActiveRequests();
        foreach (var request in requests)
        {
            GameObject newGo = Instantiate(itemPrefab, listParent);
            RescueItemUI itemScript = newGo.GetComponent<RescueItemUI>();
            itemScript.Setup(request, rescueManager, this);
        }
    }

    public void RefreshAcceptButtons()
    {
        if (!gameObject.activeSelf || listParent == null) return;

        foreach (Transform child in listParent)
        {
            RescueItemUI item = child.GetComponent<RescueItemUI>();
            item?.RefreshAcceptButton();
        }
    }
}
