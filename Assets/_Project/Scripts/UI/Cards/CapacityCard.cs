using UnityEngine;
using UnityEngine.UI;

public class CapacityCard : HUDCard
{
    [Header("Capacidad")]
    [SerializeField] private Image fillBar;

    protected override void Awake()
    {
        base.Awake();
        SetLabel("CAPACIDAD");
        SetValue("0 / 0");
    }

    public void UpdateCapacity(int current, int max)
    {
        SetValue($"{current} / {max}");

        if (fillBar != null)
            fillBar.fillAmount = (float)current / max;
    }
}