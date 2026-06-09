using UnityEngine;
using TMPro;

public class EndOfDayUI : BasePanel
{
    private int baseReward;
    private int bonus;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI animalsText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI bonusText;

    // pausesTime = true por defecto desde BasePanel (fin del día sí pausa)

    /// <summary>
    /// Abre el panel actualizando los datos ANTES de mostrarlo,
    /// lo que evita el bug del frame anterior visible.
    /// </summary>
    public override void Open()
    {
        ShowSummary(); // datos primero
        base.Open();   // activar después
    }

    private void ShowSummary()
    {
        
        int day = DayManager.Instance.CurrentDay;
        int points = ReputationManager.Instance.CurrentPoints;
        bool allDone = TaskListUI.Instance.AllTasksCompleted();
        int totalAnimals = CapacityManager.Instance.TotalAnimalsHelped;

        baseReward = points * 10;
        bonus = allDone ? Mathf.RoundToInt(baseReward * 0.25f) : 0;

        titleText.text = $"FIN DEL DÍA {day}";
        animalsText.text = $"Ayudaste a {totalAnimals} animales en total";
        pointsText.text = $"Puntos obtenidos: {points} / 100";
        rewardText.text = $"Recompensa: ${baseReward}";
        bonusText.text = allDone
            ? $"¡Bonus completista! +${bonus}"
            : "Completá todas las tareas para el bonus";
        bonusText.color = allDone
            ? new Color(0.2f, 0.6f, 0.8f, 1f)
            : new Color(0.4f, 0.4f, 0.4f, 1f);
    }

    public void StartNextDay()
    {
        MoneyManager.Instance.AddMoney(baseReward + bonus); // acá
        ReputationManager.Instance.ResetDay();
        Close();
        DayManager.Instance.StartNewDay();
    }
}