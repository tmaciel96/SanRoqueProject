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
    public int Remaining => Mathf.Max(0, _required - _current);

    private int _reputationPoints;
    private int _current;
    private int _required;
    private FontStyles _baseTaskLabelStyle;
    private bool _hasBaseTaskLabelStyle;
    private GameObject _checkMarkGraphic;

    private const float CheckboxSize = 18f;
    private const float CheckStrokeThickness = 3f;

    private static readonly Color CheckboxFillColor = new Color(0.98f, 0.89f, 0.63f, 0.25f);
    private static readonly Color CheckboxBorderColor = new Color(0.36f, 0.2f, 0.08f, 1f);
    private static readonly Color CheckboxCheckColor = new Color(0.1f, 0.38f, 0.12f, 1f);

    public void Setup(TaskData data)
    {
        TaskId = data.id;
        _reputationPoints = data.reputationPoints;
        _current = data.current;
        _required = data.required;

        if (!_hasBaseTaskLabelStyle)
        {
            _baseTaskLabelStyle = taskLabel.fontStyle;
            _hasBaseTaskLabelStyle = true;
        }

        EnsureCheckboxVisuals();
        taskLabel.text = data.label;
        taskLabel.fontStyle = _baseTaskLabelStyle;

        UpdateProgress(data.current);
    }

    public void UpdateProgress(int current)
    {
        _current = current;
        progressText.text = $"{_current}/{_required}";

        bool done = IsCompleted;
        taskLabel.fontStyle = _baseTaskLabelStyle;

        if (_checkMarkGraphic != null)
            _checkMarkGraphic.SetActive(done);
    }

    public int GetReputationPoints() => _reputationPoints;

    private void EnsureCheckboxVisuals()
    {
        if (accentDot == null)
            return;

        accentDot.color = CheckboxFillColor;
        accentDot.raycastTarget = false;
        accentDot.rectTransform.sizeDelta = new Vector2(CheckboxSize, CheckboxSize);

        Outline outline = accentDot.GetComponent<Outline>();
        if (outline == null)
            outline = accentDot.gameObject.AddComponent<Outline>();

        outline.effectColor = CheckboxBorderColor;
        outline.effectDistance = new Vector2(1f, -1f);
        outline.useGraphicAlpha = false;

        LayoutElement layoutElement = accentDot.GetComponent<LayoutElement>();
        if (layoutElement == null)
            layoutElement = accentDot.gameObject.AddComponent<LayoutElement>();

        layoutElement.minWidth = CheckboxSize;
        layoutElement.minHeight = CheckboxSize;
        layoutElement.preferredWidth = CheckboxSize;
        layoutElement.preferredHeight = CheckboxSize;

        DisableLegacyTextCheckMarks();

        if (_checkMarkGraphic == null)
        {
            Transform existing = accentDot.transform.Find("CheckMarkGraphic");
            _checkMarkGraphic = existing != null
                ? existing.gameObject
                : CreateCheckMarkGraphic();
        }
    }

    private void DisableLegacyTextCheckMarks()
    {
        TMP_Text[] legacyTextChecks = accentDot.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text legacyTextCheck in legacyTextChecks)
        {
            if (legacyTextCheck != null)
                legacyTextCheck.gameObject.SetActive(false);
        }
    }

    private GameObject CreateCheckMarkGraphic()
    {
        GameObject checkObject = new GameObject("CheckMarkGraphic", typeof(RectTransform));
        checkObject.transform.SetParent(accentDot.transform, false);

        RectTransform rect = checkObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        CreateCheckStroke(checkObject.transform, "ShortStroke", new Vector2(-3f, -1.5f), new Vector2(7f, CheckStrokeThickness), -45f);
        CreateCheckStroke(checkObject.transform, "LongStroke", new Vector2(3f, 1f), new Vector2(12f, CheckStrokeThickness), 45f);

        return checkObject;
    }

    private void CreateCheckStroke(Transform parent, string objectName, Vector2 anchoredPosition, Vector2 size, float rotationZ)
    {
        GameObject strokeObject = new GameObject(objectName, typeof(RectTransform));
        strokeObject.transform.SetParent(parent, false);

        RectTransform rect = strokeObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
        rect.localRotation = Quaternion.Euler(0f, 0f, rotationZ);

        Image stroke = strokeObject.AddComponent<Image>();
        stroke.color = CheckboxCheckColor;
        stroke.raycastTarget = false;
    }
}
