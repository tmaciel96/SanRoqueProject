using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndOfDayUI : BasePanel
{
    private int baseReward;
    private int bonus;
    private bool isGameOver;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI animalsText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI bonusText;
    [SerializeField] private Button nextDayButton;
    [SerializeField] private TextMeshProUGUI nextDayButtonText;

    [Header("Game Over")]
    [SerializeField] private ShopItemData[] shopItems;
    [SerializeField] private int foodCost = 100;
    [SerializeField] private int medicineCost = 100;
    [SerializeField] private int toysCost = 200;

    private const string BuyFoodTaskId = "comprar_comida";
    private const string BuyMedicineTaskId = "comprar_medicina";
    private const string BuyToysTaskId = "comprar_juguetes";

    protected override void Awake()
    {
        base.Awake();

        if (nextDayButton == null)
            nextDayButton = GetComponentInChildren<Button>(true);

        if (nextDayButtonText == null && nextDayButton != null)
            nextDayButtonText = nextDayButton.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public override void Open()
    {
        ShowSummary();
        base.Open();

        if (isGameOver)
            GameManager.ChangeState(GameState.GameOver);
    }

    private void ShowSummary()
    {
        int day = DayManager.Instance.CurrentDay;
        int points = ReputationManager.Instance.CurrentPoints;
        bool allDone = TaskListUI.Instance.AllTasksCompleted();
        int totalAnimals = CapacityManager.Instance.TotalAnimalsHelped;

        baseReward = points * 10;
        bonus = allDone ? Mathf.RoundToInt(baseReward * 0.25f) : 0;

        int projectedMoney = MoneyManager.Instance.CurrentMoney + baseReward + bonus;
        isGameOver = ShouldGameOver(projectedMoney);

        titleText.text = isGameOver ? "GAME OVER" : $"FIN DEL DÍA {day}";
        animalsText.text = $"Ayudaste a {totalAnimals} animales en total";
        pointsText.text = $"Puntos obtenidos: {points} / 100";
        rewardText.text = $"Recompensa: ${baseReward}";

        bonusText.text = isGameOver
            ? "No quedan dinero, recursos ni espacio para seguir"
            : allDone
                ? $"¡Bonus completista! +${bonus}"
                : "Completá todas las tareas para el bonus";

        bonusText.color = isGameOver
            ? new Color(0.75f, 0.15f, 0.12f, 1f)
            : allDone
                ? new Color(0.2f, 0.6f, 0.8f, 1f)
                : new Color(0.4f, 0.4f, 0.4f, 1f);

        if (nextDayButton != null)
            nextDayButton.interactable = !isGameOver;

        if (nextDayButtonText != null)
            nextDayButtonText.text = isGameOver ? "GAME OVER" : "SIGUIENTE DÍA";
    }

    private bool ShouldGameOver(int projectedMoney)
    {
        bool earnedNoMoneyToday = baseReward + bonus <= 0;
        return earnedNoMoneyToday && !CanMakeProgress(projectedMoney);
    }

    private bool CanMakeProgress(int projectedMoney)
    {
        TaskListUI tasks = TaskListUI.Instance;
        CapacityManager capacity = CapacityManager.Instance;

        if (tasks == null) return false;

        bool canBuyShelter = capacity != null && capacity.CanAffordNextShelter(projectedMoney);
        int freeSpace = capacity != null ? capacity.FreeSpace : 0;
        int currentFoodCost = GetItemCost(InventoryItemType.Food, foodCost);
        int currentMedicineCost = GetItemCost(InventoryItemType.Medicine, medicineCost);
        int currentToysCost = GetItemCost(InventoryItemType.Toy, toysCost);

        if (CanCompletePurchaseTask(BuyFoodTaskId, currentFoodCost, projectedMoney)) return true;
        if (CanCompletePurchaseTask(BuyMedicineTaskId, currentMedicineCost, projectedMoney)) return true;
        if (CanCompletePurchaseTask(BuyToysTaskId, currentToysCost, projectedMoney)) return true;
        if (freeSpace > 0) return true;
        if (canBuyShelter) return true;

        return false;
    }

    private bool CanCompletePurchaseTask(string taskId, int unitCost, int projectedMoney)
    {
        if (unitCost <= 0) return false;

        return TaskListUI.Instance.TryGetRemainingForTask(taskId, out int remaining)
            && projectedMoney >= unitCost * remaining;
    }

    private int GetItemCost(InventoryItemType itemType, int fallbackCost)
    {
        if (shopItems != null)
        {
            foreach (ShopItemData item in shopItems)
                if (item != null && item.inventoryType == itemType)
                    return item.pricePerUnit;
        }

        foreach (ShopItemData item in Resources.FindObjectsOfTypeAll<ShopItemData>())
            if (item != null && item.inventoryType == itemType)
                return item.pricePerUnit;

        return fallbackCost;
    }

    public void StartNextDay()
    {
        if (isGameOver) return;

        MoneyManager.Instance.AddMoney(baseReward + bonus);
        ReputationManager.Instance.ResetDay();
        Close();
        DayManager.Instance.StartNewDay();
    }
}
