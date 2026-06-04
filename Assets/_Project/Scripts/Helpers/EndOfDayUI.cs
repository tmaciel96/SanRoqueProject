using UnityEngine;

public class EndOfDayUI : MonoBehaviour
{
    public void StartNextDay()
    {
        UIManager.Instance.CloseEndOfDayPanel();
        DayManager.Instance.StartNewDay();
    }
}
