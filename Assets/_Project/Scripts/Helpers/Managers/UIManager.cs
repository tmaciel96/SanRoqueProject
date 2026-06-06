using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private List<GameObject> activePanels = new List<GameObject>();
    [SerializeField] private GameObject endOfDayPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        DayManager.OnDayEnded += CloseAllPanels;
        DayManager.OnDayEnded += OpenEndOfDayPanel;
    }

    private void OnDisable()
    {
        DayManager.OnDayEnded -= CloseAllPanels;
        DayManager.OnDayEnded -= OpenEndOfDayPanel;
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        if (!activePanels.Contains(panel))
        {
            activePanels.Add(panel);
        }
        GameManager.ChangeState(GameState.InMenu);
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        activePanels.Remove(panel);

        if (activePanels.Count == 0)
        {
            GameManager.ChangeState(GameState.Playing);
        }
    }

    public void CloseAllPanels()
    {
        foreach (var panel in activePanels)
        {
            if (panel != null) 
            {
                panel.SetActive(false);
            }
        }
        activePanels.Clear();
        GameManager.ChangeState(GameState.Playing);
    }

    public void OpenEndOfDayPanel()
    {
        endOfDayPanel.SetActive(true);
        GameManager.ChangeState(GameState.InMenu);
    }

    public void CloseEndOfDayPanel()
    {
        endOfDayPanel.SetActive(false);
        GameManager.ChangeState(GameState.Playing);
    }
}