using UnityEngine;
using System;
using TMPro;

public class DayManager : MonoBehaviour
{
    public static event Action OnDayStarted;
    public static event Action OnDayEnded;
    public static event Action<int> OnHourChanged; // hora actual 9-18

    [Header("Timer Settings")]
    [SerializeField] private float dayDurationInSeconds = 120f;
    
    [Header("UI References")]
    [SerializeField] private TimeCard timeCard;

    private int currentDay = 0;
    private float currentTime;
    private bool isTimerRunning;
    private int lastHour = -1;

    private readonly DateTime startHour = DateTime.Today.AddHours(9); 
    private readonly DateTime endHour = DateTime.Today.AddHours(18);  

    public static DayManager Instance { get; private set; }

    public int CurrentDay => currentDay;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

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

    public void StartNewDay()
    {
        currentDay++;
        currentTime = dayDurationInSeconds;
        lastHour = -1; // reset para que dispare la hora 9 al inicio del día
        Time.timeScale = 1f;
        
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
        OnDayEnded?.Invoke();
    }

    private void UpdateTimerUI()
    {
        float progress = 1f - (currentTime / dayDurationInSeconds);
        TimeSpan totalGameTime = endHour - startHour;
        DateTime currentGameTime = startHour.AddSeconds(totalGameTime.TotalSeconds * progress);

        int hour = currentGameTime.Hour;
        int minute = currentGameTime.Minute;

        // Dispará el evento cuando cambia la hora
        if (hour != lastHour)
        {
            lastHour = hour;
            OnHourChanged?.Invoke(hour);
        }

        timeCard.UpdateTime(currentDay, hour, minute);
    }

    private void UpdateDayUI() { } // ya no hace falta, UpdateTimerUI maneja todo
}