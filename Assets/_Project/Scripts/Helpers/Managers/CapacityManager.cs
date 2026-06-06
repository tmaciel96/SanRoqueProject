using UnityEngine;
using System.Collections.Generic;

public class CapacityManager : MonoBehaviour
{
    public static CapacityManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CapacityCard capacityCard;

    [Header("Configuración")]
    [SerializeField] private int maxShelters = 6;

    private List<int> _shelterLevels = new List<int>(); // nivel de cada corral
    private int _currentAnimals = 0;
    public int TotalAnimalsHelped { get; private set; }

    public int CurrentAnimals => _currentAnimals;
    public int MaxCapacity => CalculateMaxCapacity();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Primer corral desbloqueado por defecto
        _shelterLevels.Add(1);
        RefreshUI();
    }

    public bool TryAddAnimal()
    {
        if (_currentAnimals >= MaxCapacity) return false;
        _currentAnimals++;
        TotalAnimalsHelped++;
        RefreshUI();
        return true;
    }

    public void RemoveAnimal()
    {
        if (_currentAnimals <= 0) return;
        _currentAnimals--;
        RefreshUI();
    }

    public bool TryUnlockShelter()
    {
        if (_shelterLevels.Count >= maxShelters) return false;
        _shelterLevels.Add(1);
        RefreshUI();
        TaskManager.Instance.ReportShelterExpansion();
        return true;
    }

    public bool TryUpgradeShelter(int shelterIndex)
    {
        if (shelterIndex < 0 || shelterIndex >= _shelterLevels.Count) return false;
        if (_shelterLevels[shelterIndex] >= 5) return false;
        _shelterLevels[shelterIndex]++;
        RefreshUI();
        return true;
    }

    private int CalculateMaxCapacity()
    {
        int total = 0;
        foreach (int level in _shelterLevels)
            total += AnimalsForLevel(level);
        return total;
    }

    private int AnimalsForLevel(int level)
    {
        return level switch
        {
            1 => 1,
            2 => 1,
            3 => 2,
            4 => 2,
            5 => 3,
            _ => 0
        };
    }

    private void RefreshUI()
    {
        capacityCard.UpdateCapacity(_currentAnimals, MaxCapacity);
    }
}