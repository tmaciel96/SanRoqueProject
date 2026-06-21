using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }
    public static event System.Action<IReadOnlyList<TaskData>> OnDayTasksGenerated;

    [Header("Datos")]
    [SerializeField] private TaskDatabase database;

    [Header("Referencias")]
    [SerializeField] private TaskListUI taskListUI;

    [Header("Configuración")]
    [SerializeField] private int maxShelterExpansions = 6;
    [SerializeField] private int shelterTaskEveryNDays = 2; // cada cuántos días aparece la tarea de refugio

    private const string ShelterExpansionTaskId = "expandir";
    private const string BuyFoodTaskId = "comprar_comida";
    private const string BuyMedicineTaskId = "comprar_medicina";
    private const string BuyToysTaskId = "comprar_juguetes";
    private const string RescueTaskId = "rescatar";
    private const string PettingTaskId = "dar_mimos";
    private const string GiveFoodTaskId = "alimentar";
    private const string GiveWaterTaskId = "dar_agua";
    private const string GiveMediceTaskId = "curar";





    // TODO: cuando haya prefabs de mejora de nivel, agregar const string ShelterUpgradeTaskId = "mejorar_refugio";

    private int _currentDay = 1;
    private int _shelterExpansions = 0;
    private readonly Dictionary<string, int> _taskProgress = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable() => DayManager.OnDayStarted += OnDayStarted;
    private void OnDisable() => DayManager.OnDayStarted -= OnDayStarted;

    private void OnDayStarted()
    {
        _currentDay = DayManager.Instance.CurrentDay;
        _taskProgress.Clear();
        var tasks = GenerateDayTasks();
        taskListUI.SetTasks(tasks);
        OnDayTasksGenerated?.Invoke(tasks);
    }

    private List<TaskData> GenerateDayTasks()
    {
        var result = new List<TaskData>();

        var categories = new List<TaskCategory>
        {
            TaskCategory.Cuidado,
            TaskCategory.Rescate,
            TaskCategory.Tienda
        };

        // Tarea de refugio aparece cada N días y solo si quedan expansiones disponibles
        bool esDiaDeRefugio = _currentDay % 2 != 0; // días impares: 1, 3, 5, 7...
        bool quedanExpansiones = _shelterExpansions < maxShelterExpansions;

        if (esDiaDeRefugio && quedanExpansiones)
        {
            categories.Add(TaskCategory.Refugio);
            CapacityManager.Instance.EnableExpansion();
        }
        // No llamamos DisableExpansion acá — se deshabilita solo cuando se compra

        // TODO: cuando haya prefabs de mejora, agregar lógica para días impares:
        // bool esDiaDeMejora = !esDiaDeRefugio && TieneCorralesParaMejorar();
        // if (esDiaDeMejora) categories.Add(TaskCategory.RefugioMejora);

        foreach (var category in categories)
        {
            if (result.Count >= 5) break;

            var pool = database.GetByCategory(category);
            if (pool.Count == 0) continue;

            TaskDefinition picked = pool[Random.Range(0, pool.Count)];
            result.Add(ToTaskData(picked));
        }

        // Rellenar hasta 5 con tareas de Cuidado sin duplicar
        while (result.Count < 5)
        {
            var pool = database.GetByCategory(TaskCategory.Cuidado);
            if (pool.Count == 0) break;

            TaskDefinition picked = pool[Random.Range(0, pool.Count)];
            if (!result.Exists(t => t.id == picked.id))
                result.Add(ToTaskData(picked));
        }

        return result;
    }

    private TaskData ToTaskData(TaskDefinition def)
    {
        return new TaskData
        {
            id = def.id,
            label = def.label,
            current = 0,
            required = def.GetRequired(_currentDay),
            accentColor = def.accentColor,
            reputationPoints = def.reputationPoints
        };
    }

    public void ReportProgress(string taskId, int current)
    {
        _taskProgress[taskId] = current;
        taskListUI.UpdateTask(taskId, current);
    }

    public void AddProgress(string taskId, int amount = 1)
    {
        if (string.IsNullOrEmpty(taskId) || amount <= 0) return;

        _taskProgress.TryGetValue(taskId, out int current);
        ReportProgress(taskId, current + amount);
    }

    public void ReportShelterExpansion()
    {
        _shelterExpansions++;

        // Solo actualiza la UI de tarea si hay una tarea de refugio activa este día
        bool esDiaDeRefugio = _currentDay % 2 != 0;
        if (esDiaDeRefugio)
            AddProgress(ShelterExpansionTaskId);

        if (_shelterExpansions >= maxShelterExpansions)
            Debug.Log("Refugio completo — no se generarán más tareas de expansión.");
    }

    public void ReportShopPurchase(InventoryItemType itemType, int amount = 1)
    {
        string taskId = itemType switch
        {
            InventoryItemType.Food => BuyFoodTaskId,
            InventoryItemType.Medicine => BuyMedicineTaskId,
            InventoryItemType.Toy => BuyToysTaskId,
            _ => null
        };

        AddProgress(taskId, amount);
    }

    public void ReportRescue(int amount = 1)
    {
        AddProgress(RescueTaskId, amount);
    }

    public void ReportCare(CareType careType){

        string taskId = careType switch
        {
            CareType.Food => GiveFoodTaskId,
            CareType.Water => GiveWaterTaskId,
            CareType.Petting => PettingTaskId,
            CareType.Medicine => GiveMediceTaskId,
            _ => null
        };

        AddProgress(taskId);
    }

    // TODO: cuando haya prefabs de mejora, implementar:
    // public void ReportShelterUpgrade()
    // {
    //     taskListUI.UpdateTask(ShelterUpgradeTaskId, 1);
    // }
}
