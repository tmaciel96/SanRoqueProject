using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskItemUI : MonoBehaviour
{
    [SerializeField] private Image accentDot;
    [SerializeField] private TMP_Text taskLabel;
    [SerializeField] private TMP_Text progressText;

    public string TaskId { get; private set; }
    private int _reputationPoints;

    public void Setup(TaskData data)
    {
        TaskId = data.id;
        taskLabel.text = data.label;
        accentDot.color = data.accentColor;
        _reputationPoints = data.reputationPoints;
        UpdateProgress(data.current, data.required);
    }

    public void UpdateProgress(int current, int required)
    {
        progressText.text = $"{current}/{required}";
        bool done = current >= required;
        taskLabel.fontStyle = done ? FontStyles.Strikethrough : FontStyles.Normal;
        accentDot.color = new Color(accentDot.color.r, accentDot.color.g, accentDot.color.b, done ? 0.4f : 1f);
    }

    public int GetReputationPoints() => _reputationPoints;
}