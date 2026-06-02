using UnityEngine;

public class MoneyCard : HUDCard
{
    public void SetMoney(float amount)
    {
        SetValue("$" + amount.ToString("0"));
    }
}