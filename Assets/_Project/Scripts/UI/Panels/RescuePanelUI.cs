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
        base.Awake(); // auto-registro en UIManager
    }

    private void OnEnable()
    {
        RescueManager.OnRequestsUpdated += RefreshList;
    }

    private void OnDisable()
    {
        RescueManager.OnRequestsUpdated -= RefreshList;
    }

    public override void Open()
    {
        base.Open();
        RefreshList();
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
            itemScript.Setup(request, rescueManager);
        }
    }
}