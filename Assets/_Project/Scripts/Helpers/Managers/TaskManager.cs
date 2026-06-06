using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [Header("Datos")]
    [SerializeField] private TaskDatabase database;

    [Header("Referencias")]
    [SerializeField] private TaskListUI taskListUI;

    [Header("Configuración")]
    [SerializeField] private int maxShelterExpansions = 6;

    private int _currentDay = 1;
    private int _shelterExpansions = 0;

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
        var tasks = GenerateDayTasks();
        taskListUI.SetTasks(tasks);
    }

    private List<TaskData> GenerateDayTasks()
    {
        var result = new List<TaskData>();

        // Categorías disponibles
        var categories = new List<TaskCategory> 
        { 
            TaskCategory.Cuidado, 
            TaskCategory.Rescate, 
            TaskCategory.Tienda 
        };

        // Refugio solo disponible si no se completaron todas las expansiones
        if (_shelterExpansions < maxShelterExpansions)
            categories.Add(TaskCategory.Refugio);

        // 1 tarea aleatoria por categoría disponible, hasta 5 tareas
        foreach (var category in categories)
        {
            if (result.Count >= 5) break;

            var pool = database.GetByCategory(category);
            if (pool.Count == 0) continue;

            TaskDefinition picked = pool[Random.Range(0, pool.Count)];
            result.Add(ToTaskData(picked));
        }

        // Si quedaron menos de 5 por categoría bloqueada, rellenamos con Cuidado
        while (result.Count < 5)
        {
            var pool = database.GetByCategory(TaskCategory.Cuidado);
            TaskDefinition picked = pool[Random.Range(0, pool.Count)];

            // Evitar duplicados
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
        taskListUI.UpdateTask(taskId, current);
    }

    public void ReportShelterExpansion()
    {
        _shelterExpansions++;
        if (_shelterExpansions >= maxShelterExpansions)
            Debug.Log("Refugio completo — categoría Refugio deshabilitada");
    }
}