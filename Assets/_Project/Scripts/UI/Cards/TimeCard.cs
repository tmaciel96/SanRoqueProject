public class TimeCard : HUDCard
{
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
    }
}