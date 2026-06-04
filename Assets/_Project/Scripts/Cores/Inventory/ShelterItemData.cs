using UnityEngine;

public enum ShelterItemTargetType
{
    Animal,
    Pen,
    Custom
}

[CreateAssetMenu(fileName = "ShelterItemData", menuName = "San Roque/Inventory/Item Data")]
public class ShelterItemData : ScriptableObject
{
    [Header("UI")]
    [SerializeField] private string displayName = "Item";
    [SerializeField] private Sprite icon;

    [Header("Shop")]
    [SerializeField] private int cost = 100;
    [SerializeField] private int maxInventory = 10;
    [SerializeField] private ShelterItemTargetType targetType = ShelterItemTargetType.Animal;

    [Header("Animal Effects")]
    [SerializeField] private float hungerReduction;
    [SerializeField] private float happinessIncrease;
    [SerializeField] private float healthIncrease;

    [Header("Pen Effects")]
    [SerializeField] private int penUpgradeAmount = 1;

    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public int Cost => Mathf.Max(0, cost);
    public int MaxInventory => Mathf.Max(1, maxInventory);
    public ShelterItemTargetType TargetType => targetType;
    public float HungerReduction => Mathf.Max(0f, hungerReduction);
    public float HappinessIncrease => Mathf.Max(0f, happinessIncrease);
    public float HealthIncrease => Mathf.Max(0f, healthIncrease);
    public int PenUpgradeAmount => Mathf.Max(1, penUpgradeAmount);

    public void ConfigureRuntimeDefaults(
        string itemName,
        int itemCost,
        int itemMaxInventory,
        ShelterItemTargetType itemTargetType,
        float itemHungerReduction = 0f,
        float itemHappinessIncrease = 0f,
        float itemHealthIncrease = 0f,
        int itemPenUpgradeAmount = 1)
    {
        displayName = itemName;
        cost = itemCost;
        maxInventory = itemMaxInventory;
        targetType = itemTargetType;
        hungerReduction = itemHungerReduction;
        happinessIncrease = itemHappinessIncrease;
        healthIncrease = itemHealthIncrease;
        penUpgradeAmount = itemPenUpgradeAmount;
    }
}
