using UnityEngine;
using System;
using System.Collections.Generic;

public class CapacityManager : MonoBehaviour
{
    public static event Action OnCapacityChanged;
    public static CapacityManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CapacityCard capacityCard;

    [Header("Configuración")]
    [SerializeField] private ShelterDatabase shelterDatabase;

    private readonly List<int> _unlockedTiers = new List<int>();
    private int _currentAnimals = 0;
    private int _initialCapacity = 0; // sin corral gratis, el jugador compra el primero
    private bool _expansionAvailable = false; // solo true cuando hay tarea activa

    public int CurrentAnimals => _currentAnimals;
    public int MaxCapacity => CalculateMaxCapacity();
    public int FreeSpace => Mathf.Max(0, MaxCapacity - _currentAnimals);
    public int TotalAnimalsHelped { get; private set; }
    public int UnlockedCount => _unlockedTiers.Count;
    public bool ExpansionAvailable => _expansionAvailable;
    public bool HasShelter => _unlockedTiers.Count > 0; // false hasta que compre el primero
    public bool HasAvailableSpace => HasShelter && _currentAnimals < MaxCapacity;

    [Header("Referencias")]
    [SerializeField] private ShelterGridManager shelterGridManager;

    // ── Control de expansión (llamado por TaskManager) ────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        RefreshUI();
    }

    public void EnableExpansion()
    {
        _expansionAvailable = true;
        shelterGridManager?.ShowNextAvailable();
        Debug.Log("CapacityManager: expansión habilitada para este día.");
    }

    public void DisableExpansion()
    {
        _expansionAvailable = false;
    }

    // ── Animales ──────────────────────────────────────────────────────────

    public void AssignAnimalToPen(RescueRequest completedRequest)
    {
        if (_currentAnimals >= MaxCapacity) {
            Debug.LogError("[CapacityManager] No se pudo asignar al animal a un corral despues de haber aceptado un rescate y esperado su llegada.");
            return;
        }
                  
        Pen pen = shelterGridManager.GetFirstEmptyPen();
        pen.PlaceAnimal(completedRequest.animalData);

        _currentAnimals++;
        TotalAnimalsHelped++;
        RefreshUI();
    }

    public int GetNextShelterCost()
    {
        if (shelterDatabase == null) return -1;

        int nextIndex = _unlockedTiers.Count;
        if (nextIndex >= shelterDatabase.TierCount) return -1;

        ShelterTierData tier = shelterDatabase.GetTier(nextIndex);
        return tier != null ? tier.unlockCost : -1;
    }

    public bool CanAffordNextShelter(int availableMoney)
    {
        int nextCost = GetNextShelterCost();
        return _expansionAvailable && nextCost >= 0 && availableMoney >= nextCost;
    }

    public void RemoveAnimal()
    {
        if (_currentAnimals <= 0) return;
        _currentAnimals--;
        RefreshUI();
    }

    // ── Desbloqueo de corrales ────────────────────────────────────────────

    public bool TryUnlockNextShelter(ShelterGridManager gridManager, int row, int column)
    {
        if (!_expansionAvailable)
        {
            Debug.Log("CapacityManager: no hay tarea de expansión activa hoy.");
            return false;
        }

        int nextTierIndex = _unlockedTiers.Count; // empieza en 0, tier 0 = $100

        if (nextTierIndex >= shelterDatabase.TierCount)
        {
            Debug.Log("CapacityManager: refugio completo.");
            return false;
        }

        ShelterTierData tier = shelterDatabase.GetTier(nextTierIndex);
        if (tier == null) return false;

        if (!MoneyManager.Instance.TrySpend(tier.unlockCost))
        {
            Debug.Log($"CapacityManager: dinero insuficiente. Costo: ${tier.unlockCost}");
            return false;
        }

        _unlockedTiers.Add(nextTierIndex);
        RefreshUI();

        // NO llamamos UnlockNeighbors acá — el siguiente pen se habilita
        // en el próximo día impar via EnableExpansion() → ShowNextAvailable()

        TaskManager.Instance.ReportShelterExpansion();
        DisableExpansion();

        Debug.Log($"CapacityManager: desbloqueado {tier.label}. Capacidad total: {MaxCapacity}");
        return true;
    }

    // ── Capacidad ─────────────────────────────────────────────────────────

    private int CalculateMaxCapacity()
    {
        int total = _initialCapacity; // corral inicial gratis
        foreach (int tierIndex in _unlockedTiers)
        {
            ShelterTierData tier = shelterDatabase.GetTier(tierIndex);
            if (tier != null)
                total += tier.animalCapacity;
        }
        return total;
    }

    // ── Helpers para Pen ──────────────────────────────────────────────────

    public string GetNextUnlockCostText()
    {
        int nextIndex = _unlockedTiers.Count;
        if (nextIndex >= shelterDatabase.TierCount) return "";
        ShelterTierData tier = shelterDatabase.GetTier(nextIndex);
        return tier != null ? $"${tier.unlockCost}" : "";
    }

    public string GetTierCapacityText(int row, int column)
    {
        int tierIndex = _unlockedTiers.Count - 1;
        if (tierIndex < 0) return $"0/{_initialCapacity}";
        ShelterTierData tier = shelterDatabase.GetTier(tierIndex);
        return tier != null ? $"0/{tier.animalCapacity}" : "0/0";
    }

    // ── UI ────────────────────────────────────────────────────────────────

    private void RefreshUI()
    {
        capacityCard?.UpdateCapacity(_currentAnimals, MaxCapacity);
        OnCapacityChanged?.Invoke();
    }
}
