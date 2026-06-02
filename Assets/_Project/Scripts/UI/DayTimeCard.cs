using UnityEngine;

public class DayTimeCard : HUDCard
{
    public void SetDay(float day)
    {
        SetLabel("Día " + day.ToString("0"));
    }

    public void SetTime(float time)
    {
        SetValue(time.ToString("0"));
    }
}