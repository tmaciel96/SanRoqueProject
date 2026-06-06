using UnityEngine;

public class MoneyCard : HUDCard
{
    private int _current = 0;

    [Header("Colores")]
    [SerializeField] private Color gainColor = new Color(0.4f, 1f, 0.4f);
    [SerializeField] private Color lossColor = new Color(1f, 0.4f, 0.4f);

    protected override void Awake()
    {
        base.Awake();
        SetLabel("DINERO");
        SetValue("$00");
    }

    public void UpdateMoney(int newAmount)
    {
        bool increased = newAmount > _current;
        _current = newAmount;
        SetValue($"${newAmount:N0}");
        FlashValue(increased ? gainColor : lossColor);
    }
}