using UnityEngine;
using System;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Configuración")]
    [SerializeField] private int startingMoney = 500;

    [Header("UI")]
    [SerializeField] private MoneyCard moneyCard;

    public int CurrentMoney { get; private set; }

    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        CurrentMoney = startingMoney;
        moneyCard.UpdateMoney(CurrentMoney);
    }

    public bool TrySpend(int amount)
    {
        if (amount > CurrentMoney) return false;
        CurrentMoney -= amount;
        Refresh();
        return true;
    }

    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
        Refresh();
    }

    private void Refresh()
    {
        moneyCard.UpdateMoney(CurrentMoney);
        OnMoneyChanged?.Invoke(CurrentMoney);
    }
}