using UnityEngine;
using System;
using TMPro;

public class DayManager : MonoBehaviour
{
    public static event Action OnDayStarted;
    public static event Action OnDayEnded;

    [Header("Timer Settings")]
    [SerializeField] private float dayDurationInSeconds = 120f;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject endOfDayPanel;

    private int currentDay = 1;
    private float currentTime;
    private bool isTimerRunning;

    private readonly DateTime startHour = DateTime.Today.AddHours(9); 
    private readonly DateTime endHour = DateTime.Today.AddHours(18);  

    void Start()
    {
        StartNewDay();
    }

    void Update()
    {
        if (!isTimerRunning) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            EndDay();
        }
    }

    private void StartNewDay()
    {
        currentTime = dayDurationInSeconds;
        endOfDayPanel.SetActive(false);
        Time.timeScale = 1f;
        
        UpdateDayUI();
        UpdateTimerUI();
        
        isTimerRunning = true;
        OnDayStarted?.Invoke();
    }

    private void EndDay()
    {
        currentTime = 0;
        isTimerRunning = false;
        UpdateTimerUI();
        
        Time.timeScale = 0f;
        endOfDayPanel.SetActive(true);
        OnDayEnded?.Invoke();
    }

    public void OnAcceptEndDayButtonPressed()
    {
        currentDay++;
        StartNewDay();
    }

    private void UpdateDayUI()
    {
        dayText.text = $"Día {currentDay}";
    }

    private void UpdateTimerUI()
    {
        float progress = 1f - (currentTime / dayDurationInSeconds);
        TimeSpan totalGameTime = endHour - startHour;
        DateTime currentGameTime = startHour.AddSeconds(totalGameTime.TotalSeconds * progress);

        timerText.text = currentGameTime.ToString("hh:mm tt").ToLower();
    }
}