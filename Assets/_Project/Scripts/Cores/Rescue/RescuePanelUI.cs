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
        panelContainer.SetActive(true);
        RefreshList();
    }

    public void ClosePanel()
    {
        panelContainer.SetActive(false);
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