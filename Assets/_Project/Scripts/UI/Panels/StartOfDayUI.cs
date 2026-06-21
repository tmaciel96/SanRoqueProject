using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartOfDayUI : BasePanel
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private TextMeshProUGUI tasksText;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI continueButtonText;

    protected override void Awake()
    {
        base.Awake();

        if (continueButton == null)
            continueButton = GetComponentInChildren<Button>(true);

        if (continueButtonText == null && continueButton != null)
            continueButtonText = continueButton.GetComponentInChildren<TextMeshProUGUI>(true);

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(Close);
            continueButton.onClick.AddListener(Close);
        }
    }

    protected override void OnDestroy()
    {
        if (continueButton != null)
            continueButton.onClick.RemoveListener(Close);

        base.OnDestroy();
    }

    public void Open(IReadOnlyList<TaskData> tasks)
    {
        Populate(tasks);
        Time.timeScale = 0f;
        base.Open();
    }

    public override void Close()
    {
        base.Close();
        Time.timeScale = 1f;
    }

    private void Populate(IReadOnlyList<TaskData> tasks)
    {
        int day = DayManager.Instance != null ? DayManager.Instance.CurrentDay : 1;

        if (titleText != null)
            titleText.text = "BIENVENIDO AL REFUGIO";

        if (dayText != null)
            dayText.text = $"DIA {day}";

        if (subtitleText != null)
            subtitleText.text = "Estas son tus tareas del dia";

        if (continueButtonText != null)
            continueButtonText.text = "COMENZAR DIA";

        if (tasksText == null)
            return;

        if (tasks == null || tasks.Count == 0)
        {
            tasksText.text = "- No hay tareas asignadas";
            return;
        }

        var builder = new StringBuilder();
        for (int i = 0; i < tasks.Count; i++)
        {
            TaskData task = tasks[i];
            if (task == null)
                continue;

            builder.Append("- ");
            builder.Append(task.label);
            builder.Append(": ");
            builder.Append(task.current);
            builder.Append("/");
            builder.Append(task.required);

            if (i < tasks.Count - 1)
                builder.AppendLine();
        }

        tasksText.text = builder.ToString();
    }
}
