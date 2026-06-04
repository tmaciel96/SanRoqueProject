using UnityEngine;
using System.Collections.Generic;

public class RescuePanelUI : MonoBehaviour
{
    [SerializeField] private RescueManager rescueManager;
    [SerializeField] private GameObject panelContainer;
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject itemPrefab;

    private void OnEnable()
    {
        RescueManager.OnRequestsUpdated += RefreshList;
    }

    private void OnDisable()
    {
        RescueManager.OnRequestsUpdated -= RefreshList;
    }

    public void OpenPanel()
    {
        UIManager.Instance.OpenPanel(panelContainer);
        RefreshList();
    }

    public void ClosePanel()
    {
        UIManager.Instance.ClosePanel(panelContainer);
    }

    private void RefreshList()
    {
        if (!panelContainer.activeSelf) return;

        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject);
        }

        List<RescueRequest> requests = rescueManager.GetActiveRequests();
        foreach (var request in requests)
        {
            GameObject newGo = Instantiate(itemPrefab, listParent);
            RescueItemUI itemScript = newGo.GetComponent<RescueItemUI>();
            itemScript.Setup(request, rescueManager);
        }
    }
}