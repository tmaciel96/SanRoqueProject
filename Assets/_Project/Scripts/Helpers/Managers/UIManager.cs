using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // Paneles actualmente abiertos
    private readonly List<BasePanel> activePanels = new List<BasePanel>();

    // Registro de todos los paneles disponibles en la escena, indexados por tipo
    private readonly Dictionary<System.Type, BasePanel> availablePanels = new Dictionary<System.Type, BasePanel>();

    [Header("Click Blocker")]
    [Tooltip("Image fullscreen transparente con Raycast Target = true. Bloquea clicks al mundo cuando hay paneles abiertos.")]
    [SerializeField] private GameObject clickBlocker;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Busca todos los paneles en la escena, activos e inactivos
        foreach (var panel in Resources.FindObjectsOfTypeAll<BasePanel>())
        {
            if (panel.gameObject.scene.IsValid()) // excluye prefabs
                RegisterAvailablePanel(panel);
        }
    }

    private void OnEnable()
    {
        DayManager.OnDayEnded += CloseAllPanels;
        DayManager.OnDayEnded += OpenEndOfDayPanel;
    }

    private void OnDisable()
    {
        DayManager.OnDayEnded -= CloseAllPanels;
        DayManager.OnDayEnded -= OpenEndOfDayPanel;
    }

    // ── Auto-registro (llamado por cada BasePanel en su Awake) ───────────

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.RestartGame();
    }

    public void RegisterAvailablePanel(BasePanel panel)
    {
        System.Type type = panel.GetType();
        if (!availablePanels.ContainsKey(type))
            availablePanels[type] = panel;
    }

    public void UnregisterAvailablePanel(BasePanel panel)
    {
        availablePanels.Remove(panel.GetType());
    }

    /// <summary>
    /// Obtiene un panel disponible en la escena por tipo. Retorna null si no existe.
    /// Uso: GetPanel<EndOfDayUI>()
    /// </summary>
    public T GetPanel<T>() where T : BasePanel
    {
        if (availablePanels.TryGetValue(typeof(T), out BasePanel panel))
            return panel as T;

        Debug.LogWarning($"UIManager: no hay ningún panel de tipo {typeof(T).Name} registrado en esta escena.");
        return null;
    }

    // ── Registro de paneles abiertos (llamado por BasePanel.Open/Close) ───

    public void RegisterPanel(BasePanel panel)
    {
        if (!activePanels.Contains(panel))
            activePanels.Add(panel);

        if (clickBlocker != null)
            clickBlocker.SetActive(true);

        if (panel.PausesTime)
            GameManager.ChangeState(GameState.InMenu);
    }

    public void UnregisterPanel(BasePanel panel)
    {
        activePanels.Remove(panel);

        if (HasNoActivePanels() && clickBlocker != null)
            clickBlocker.SetActive(false);

        if (!HasPanelThatPausesTime())
            GameManager.ChangeState(GameState.Playing);
    }

    public bool HasNoActivePanels() => activePanels.Count == 0;

    private bool HasPanelThatPausesTime()
    {
        foreach (var p in activePanels)
            if (p.PausesTime) return true;
        return false;
    }

    // ── API pública ───────────────────────────────────────────────────────

    public void CloseAllPanels()
    {
        var copy = new List<BasePanel>(activePanels);
        foreach (var panel in copy)
            panel.Close();

        activePanels.Clear();

        if (clickBlocker != null)
            clickBlocker.SetActive(false);

        GameManager.ChangeState(GameState.Playing);
    }

    // ── Shortcuts tipados ─────────────────────────────────────────────────
    // Cada shortcut verifica si el panel existe antes de usarlo,
    // así no rompe si no está en la escena actual.

    public void OpenEndOfDayPanel()
    {
        GetPanel<EndOfDayUI>()?.Open();
    }

    public void CloseEndOfDayPanel()
    {
        GetPanel<EndOfDayUI>()?.Close();
    }

    public void ShowAnimalPanel(Animal animal)
    {
        GetPanel<AnimalInfoPanel>()?.Open(animal);
    }

    // ── Rescate ───────────────────────────────────────────────────────────────

    public void ShowRescuePanel()
    {
        GetPanel<RescuePanel>()?.Open();
    }

    public void CloseRescuePanel()
    {
        GetPanel<RescuePanel>()?.Close();
    }

    // ── Tienda ────────────────────────────────────────────────────────────

    public void ShowShopItemPanel(ShopItemData itemData)
    {
        GetPanel<ShopItemPanelUI>()?.Open(itemData);
    }

    public void CloseShopItemPanel()
    {
        GetPanel<ShopItemPanelUI>()?.Close();
    }
    
}
