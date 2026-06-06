using UnityEngine;

public class TimeCard : HUDCard
{
    [Header("Sprites del reloj")]
    [SerializeField] private Sprite[] clockSprites;

    protected override void Awake()
    {
        base.Awake();
        SetLabel("DÍA 1");
        SetValue("08:00 AM");
    }

    public void UpdateTime(int day, int hour, int minute)
    {
        bool isAM = hour < 12;
        int displayHour = hour % 12;
        if (displayHour == 0) displayHour = 12;

        SetLabel($"DÍA {day}");
        SetValue($"{displayHour:00}:{minute:00} {(isAM ? "AM" : "PM")}");

        int spriteIndex = hour % 12;
        if (clockSprites != null && clockSprites.Length == 12)
            SetIcon(clockSprites[spriteIndex]);
    }
}