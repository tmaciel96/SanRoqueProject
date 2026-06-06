using UnityEngine;
using TMPro;

public class EndOfDayUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI animalsText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI bonusText;

    private void OnEnable()
    {
        ShowSummary();
    }

    private void ShowSummary()
    {
    
        int day = DayManager.Instance.CurrentDay;
        int points = ReputationManager.Instance.CurrentPoints;
        bool allDone = TaskListUI.Instance.AllTasksCompleted();
        int totalAnimals = CapacityManager.Instance.TotalAnimalsHelped;

        int baseReward = points * 10;
        int bonus = allDone ? Mathf.RoundToInt(baseReward * 0.25f) : 0;
        titleText.text = $"FIN DEL DÍA {day}";
        animalsText.text = $"🐾 Ayudaste a {totalAnimals} animales en total";
        pointsText.text = $"Puntos obtenidos: {points} / 100";
        rewardText.text = $"Recompensa: ${baseReward}";
        bonusText.text = allDone
            ? $"¡Bonus completista! +${bonus}"
            : $"Completá todas las tareas para el bonus";
        bonusText.color = allDone 
            ? new Color(0.2f, 0.6f, 0.8f, 1f)
            : new Color(0.4f, 0.4f, 0.4f, 1f);

        MoneyManager.Instance.AddMoney(baseReward + bonus);
    }

    public void StartNextDay()
    {
        ReputationManager.Instance.ResetDay();
        UIManager.Instance.CloseEndOfDayPanel();
        DayManager.Instance.StartNewDay();
    }
}