using UnityEngine;
using System;
using System.Collections.Generic;

public class RescueManager : MonoBehaviour
{
    public static event Action<AnimalData> OnRescueAccepted;
    public static event Action OnRequestsUpdated;

    [Header("Settings")]
    [SerializeField] private int maxRequests = 4;

    [Header("Referencias")]
    [SerializeField] private AnimalPrefabCatalog animalPrefabCatalog;
    [SerializeField] private ShelterGridManager shelterGridManager;


    [Header("UI Component")]
    [SerializeField] private NotificationBadge rescueBadge;

    private List<RescueRequest> activeRequests = new List<RescueRequest>();
    private readonly string[] mockNames = { "Bondiola", "Feca", "Pocho", "Yuyo", "Negro", "Michi", "Bala", "Chispa", "Coco", "Pipa", "Bash", "Miga", "Pixel", "Astro", "Pampa"};
    private int _nextAnimalId = 1;

    private void OnEnable()
    {
        DayManager.OnDayStarted += HandleDayStarted;
        DayManager.OnDayEnded += HandleDayEnded;
    }

    private void OnDisable()
    {
        DayManager.OnDayStarted -= HandleDayStarted;
        DayManager.OnDayEnded -= HandleDayEnded;
    }

    public List<RescueRequest> GetActiveRequests() => activeRequests;

    private void HandleDayEnded()
    {
        foreach (var request in activeRequests)
        {
            request.daysRemaining--;
        }

        activeRequests.RemoveAll(request => request.daysRemaining <= 0);
        rescueBadge.UpdateBadge(activeRequests.Count);
        OnRequestsUpdated?.Invoke();
    }
    private void HandleDayStarted()
    {
        GenerateDailyRequests();
        rescueBadge.UpdateBadge(activeRequests.Count);
        OnRequestsUpdated?.Invoke();
    }
    private void GenerateDailyRequests()
    {
        if (activeRequests.Count >= maxRequests) return;
        float chance = UnityEngine.Random.value;
        int newRequestsCount = 0;

        if (chance < 0.50f) newRequestsCount = 1;
        else if (chance < 0.85f) newRequestsCount = 2;
        else if (chance < 0.98f) newRequestsCount = 3;
        else newRequestsCount = 4;

        for (int i = 0; i < newRequestsCount; i++)
        {
            if (activeRequests.Count >= maxRequests) break;

            AnimalType randomSpecies = (AnimalType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(AnimalType)).Length);
            
            AnimalData mockAnimal = new AnimalData
            {
                animalName = mockNames[UnityEngine.Random.Range(0, mockNames.Length)],
                species = randomSpecies,
                variantIndex = 0, //GetRandomVariantIndex(randomSpecies),
                hunger = 100f,
                thirst = 100f,
                affection = 50f,
                health = 100f
            };

            int randomDaysRemaining = UnityEngine.Random.Range(1, 4);
            activeRequests.Add(new RescueRequest(mockAnimal, randomDaysRemaining));
        }

    }

    private int GetRandomVariantIndex(AnimalType species)
    {
        // 0 -> Dog
        // 1 -> Cat
        int variantCount = animalPrefabCatalog != null
            ? animalPrefabCatalog.GetVariantCount(species)
            : 1;
        return variantCount > 1 ? UnityEngine.Random.Range(0, variantCount) : 0;
    }

    public bool AcceptRescue(RescueRequest request)
    {
        if (request == null || !activeRequests.Contains(request))
            return false;
        if (CapacityManager.Instance == null || !CapacityManager.Instance.HasAvailableSpace)
            return false;

        if (!CapacityManager.Instance.TryAddAnimal())
            return false;

        if (!TryPlaceRescuedAnimal(request.animalData))
        {
            CapacityManager.Instance.RemoveAnimal();
            return false;
        }

        activeRequests.Remove(request);
        rescueBadge.UpdateBadge(activeRequests.Count);
        OnRescueAccepted?.Invoke(request.animalData);

        return true;
    }

    public void RejectRescue(RescueRequest request)
    {
        if (request == null || !activeRequests.Remove(request))
            return;

        rescueBadge.UpdateBadge(activeRequests.Count);
    }

    private bool TryPlaceRescuedAnimal(AnimalData animalData)
    {
        if (animalPrefabCatalog == null || shelterGridManager == null || animalData == null)
        {
            Debug.LogError("RescueManager: faltan referencias para colocar el animal.");
            return false;
        }

        GameObject prefab = animalPrefabCatalog.GetPrefab(animalData.species);
        if (prefab == null)
        {
            Debug.LogError($"RescueManager: no hay prefab para {animalData.species}.");
            return false;
        }

        Pen pen = shelterGridManager.GetFirstEmptyPen();
        if (pen == null)
        {
            Debug.LogWarning("RescueManager: no hay corrales vacíos disponibles.");
            return false;
        }

        return pen.PlaceAnimal(prefab, animalData, _nextAnimalId++);
    }
}
