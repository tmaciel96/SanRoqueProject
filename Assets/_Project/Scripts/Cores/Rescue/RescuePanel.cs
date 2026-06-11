using UnityEngine;

public class RescuePanel : BasePanel
{
    [Header("Referencias internas")]
    [SerializeField] private UnityEngine.UI.Button closeButton;
    [SerializeField] private RescueDetailPanel detailPanel;

    protected override void Awake()
    {
        base.Awake();
        closeButton?.onClick.AddListener(Close);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        closeButton?.onClick.RemoveListener(Close);
    }

    private void Start()
    {
        gameObject.SetActive(true);
    }

    public override void Open()
    {
        Debug.Log($"[RescuePanel] Open()");
        base.Open();
        Debug.Log($"[RescuePanel] Post Open — activeSelf: {gameObject.activeSelf}");
        detailPanel?.ShowEmpty();
    }

    public override void Close()
    {
        Debug.Log($"[RescuePanel] Close() — stack:\n{System.Environment.StackTrace}");
        detailPanel?.ShowEmpty();
        base.Close();
    }
}