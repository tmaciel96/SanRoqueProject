using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Orquesta todo el sistema de rescate:
/// - Genera requests al inicio del día
/// - Coordina con RescueMapController para mostrar/ocultar markers
/// - Valida capacidad antes de aceptar
/// - Maneja el rescate activo (solo uno a la vez)
/// - Notifica completación para mostrar panel de recepción
/// </summary>
public class RescueManager : MonoBehaviour
{
    // ── Eventos públicos ─────────────────────────────────────────────────────

    /// Nuevos requests disponibles en el mapa
    public static event Action<List<RescueRequest>> OnPendingRequestsUpdated;

    /// El player confirmó un rescate (comienza el viaje)
    public static event Action<RescueRequest> OnRescueStarted;

    /// El rescate activo se completó → mostrar panel de recepción
    public static event Action<RescueRequest> OnRescueCompleted;

    /// Un request expiró del mapa sin ser atendido
    public static event Action<RescueRequest> OnRequestExpired;

    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Generación")]
    [SerializeField] private int maxPendingRequests = 4;

    [Header("Horas de rescate (rango aleatorio)")]
    [SerializeField] private int minRescueHours = 1;
    [SerializeField] private int maxRescueHours = 5;

    [Header("Horas hasta expiración (rango aleatorio)")]
    [SerializeField] private int minExpiryHours = 3;
    [SerializeField] private int maxExpiryHours = 8;

    [Header("Recompensa")]
    [SerializeField] private int baseReward = 100;

    [Header("Referencias")]
    [SerializeField] private RescueMapController mapController;
    [SerializeField] private NotificationBadge rescueBadge;

    // ── Estado interno ────────────────────────────────────────────────────────

    private List<RescueRequest> pendingRequests = new List<RescueRequest>();
    private RescueRequest activeRescue = null;

    public bool HasActiveRescue => activeRescue != null;
    public RescueRequest ActiveRescue => activeRescue;
    public List<RescueRequest> PendingRequests => pendingRequests;

    // Mock de nombres de animales
    private readonly string[] mockNames =
    {
        "Bondiola", "Feca", "Pocho", "Yuyo", "Negro",
        "Michi", "Bala", "Chispa", "Coco", "Pipa",
        "Bash", "Miga", "Pixel", "Astro", "Pampa"
    };

    // ── Unity ─────────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        DayManager.OnDayStarted  += HandleDayStarted;
        DayManager.OnDayEnded    += HandleDayEnded;
        DayManager.OnHourChanged += HandleHourChanged;
    }

    private void OnDisable()
    {
        DayManager.OnDayStarted  -= HandleDayStarted;
        DayManager.OnDayEnded    -= HandleDayEnded;
        DayManager.OnHourChanged -= HandleHourChanged;
    }

    // ── Handlers de día ───────────────────────────────────────────────────────

    private void HandleDayStarted()
    {
        GenerateDailyRequests();
        RefreshBadge();
        OnPendingRequestsUpdated?.Invoke(pendingRequests);
    }

    private void HandleDayEnded()
    {
        // El día terminó — solo refrescamos UI
        // El tick de horas lo maneja HandleHourChanged
        RefreshBadge();
        OnPendingRequestsUpdated?.Invoke(pendingRequests);
    }

    private void HandleHourChanged(int hour)
    {
        // Tick del rescate activo
        if (activeRescue != null)
        {
            bool completed = activeRescue.TickRescueHour();
            if (completed)
            {
                var finished = activeRescue;
                activeRescue = null;
                OnRescueCompleted?.Invoke(finished);
            }
        }

        // Tick de expiración de requests pendientes
        var expired = new List<RescueRequest>();
        foreach (var req in pendingRequests)
        {
            if (req.TickExpiryHour())
                expired.Add(req);
        }

        foreach (var req in expired)
        {
            pendingRequests.Remove(req);
            mapController.HideMarker(req);
            OnRequestExpired?.Invoke(req);
        }

        RefreshBadge();
        OnPendingRequestsUpdated?.Invoke(pendingRequests);
    }

    // ── Generación ────────────────────────────────────────────────────────────

    private void GenerateDailyRequests()
    {
        if (pendingRequests.Count >= maxPendingRequests) return;

        int newCount = RollNewRequestCount();
        List<int> availableSpawnPoints = mapController.GetAvailableSpawnIndices(pendingRequests);

        for (int i = 0; i < newCount; i++)
        {
            if (pendingRequests.Count >= maxPendingRequests) break;
            if (availableSpawnPoints.Count == 0) break;

            int randomIdx = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
            int spawnIndex = availableSpawnPoints[randomIdx];
            availableSpawnPoints.RemoveAt(randomIdx);

            var request = CreateRandomRequest(spawnIndex);
            pendingRequests.Add(request);
            mapController.ShowMarker(request);
        }
    }

    private int RollNewRequestCount()
    {
        float chance = UnityEngine.Random.value;
        if (chance < 0.50f) return 1;       // 50%
        if (chance < 0.85f) return 2;       // 35%
        if (chance < 0.98f) return 3;       // 13%
        return 4;                           // 2%
    }

    private RescueRequest CreateRandomRequest(int spawnIndex)
    {
        var animal = new AnimalData
        {
            animalName = mockNames[UnityEngine.Random.Range(0, mockNames.Length)],
            species    = (AnimalType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(AnimalType)).Length)
        };

        int rescueHours = UnityEngine.Random.Range(minRescueHours, maxRescueHours + 1);
        int expiryHours = UnityEngine.Random.Range(minExpiryHours, maxExpiryHours + 1);

        // Recompensa inversamente proporcional a la duración (rescates rápidos = más caro)
        int reward = baseReward + (maxRescueHours - rescueHours) * 50;

        return new RescueRequest(animal, expiryHours, rescueHours, reward, spawnIndex);
    }

    // ── API pública ───────────────────────────────────────────────────────────

    /// Llamado por RescueDetailPanel cuando el player confirma el rescate.
    /// Retorna false si no hay capacidad o ya hay un rescate activo.
    public bool TryAcceptRescue(RescueRequest request)
    {
        if (HasActiveRescue)
        {
            Debug.LogWarning("[RescueManager] Ya hay un rescate en progreso.");
            return false;
        }

        var cm = CapacityManager.Instance;
        bool hasSpace = cm != null && cm.HasShelter && cm.CurrentAnimals < cm.MaxCapacity;
        if (!hasSpace)
        {
            Debug.LogWarning("[RescueManager] Sin espacio en el corral.");
            return false;
        }

        pendingRequests.Remove(request);
        mapController.HideMarker(request);

        request.StartRescue();
        activeRescue = request;

        RefreshBadge();
        OnPendingRequestsUpdated?.Invoke(pendingRequests);
        OnRescueStarted?.Invoke(request);
        return true;
    }

    /// Llamado por RescueDetailPanel cuando el player rechaza.
    public void RejectRescue(RescueRequest request)
    {
        pendingRequests.Remove(request);
        mapController.HideMarker(request);
        request.Reject();

        RefreshBadge();
        OnPendingRequestsUpdated?.Invoke(pendingRequests);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void RefreshBadge()
    {
        rescueBadge?.UpdateBadge(pendingRequests.Count);
    }
}