using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskItemUI : MonoBehaviour
{
    [SerializeField] private Image accentDot;
    [SerializeField] private TMP_Text taskLabel;
    [SerializeField] private TMP_Text progressText;

    public string TaskId { get; private set; }
    public bool IsCompleted => _current >= _required;

    private int _reputationPoints;
    private int _current;
    private int _required;
    private Color _baseColor;

    public void Setup(TaskData data)
    {
        TaskId = data.id;
        _reputationPoints = data.reputationPoints;
        _baseColor = data.accentColor;
        _current = data.current;
        _required = data.required;

        taskLabel.text = data.label;
        accentDot.color = _baseColor;

        UpdateProgress(data.current);
    }

    public void UpdateProgress(int current)
    {
        _current = current;
        progressText.text = $"{_current}/{_required}";

        bool done = IsCompleted;
        taskLabel.fontStyle = done ? FontStyles.Strikethrough : FontStyles.Normal;
        accentDot.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, done ? 0.4f : 1f);
    }

    public int GetReputationPoints() => _reputationPoints;
}