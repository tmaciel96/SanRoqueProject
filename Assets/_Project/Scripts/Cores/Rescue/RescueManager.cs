using UnityEngine;
using System;
using System.Collections.Generic;

public class RescueManager : MonoBehaviour
{
    public static event Action<AnimalData> OnRescueAccepted;
    public static event Action OnRequestsUpdated;

    [Header("Settings")]
    [SerializeField] private int maxRequests = 4;
    [SerializeField] private bool mockHasCorralSpace = true;

    [Header("UI Component")]
    [SerializeField] private NotificationBadge rescueBadge;

    private List<RescueRequest> activeRequests = new List<RescueRequest>();
    private int currentDayTracker = 1;
    private int nextAnimalId = 1;

    private readonly string[] mockNames = { "Rex", "Luna", "Milo", "Oliver", "Bella", "Simba" };

    private void OnEnable()
    {
        DayManager.OnDayStarted += HandleNewDay;
    }

    private void OnDisable()
    {
        DayManager.OnDayStarted -= HandleNewDay;
    }

    public List<RescueRequest> GetActiveRequests() => activeRequests;

    private void HandleNewDay()
    {
        ProcessExpirations();
        GenerateDailyRequests();
        rescueBadge.UpdateBadge(activeRequests.Count);
        OnRequestsUpdated?.Invoke();
        currentDayTracker++;
    }

    private void ProcessExpirations()
    {
        activeRequests.RemoveAll(request => request.expirationDay <= currentDayTracker);
    }

    private void GenerateDailyRequests()
    {
        if (activeRequests.Count >= maxRequests) return;

        float chance = UnityEngine.Random.value;
        int newRequestsCount = 0;

        if (chance < 0.50f) newRequestsCount = 1;       // 50% probabilidad de 1
        else if (chance < 0.85f) newRequestsCount = 2;  // 35% probabilidad de 2
        else if (chance < 0.98f) newRequestsCount = 3;  // 13% probabilidad de 3
        else newRequestsCount = 4;                      // 2% probabilidad de 4 (Muy raro)

        for (int i = 0; i < newRequestsCount; i++)
        {
            if (activeRequests.Count >= maxRequests) break;

            AnimalData mockAnimal = new AnimalData();
            string randomName = mockNames[UnityEngine.Random.Range(0, mockNames.Length)];
            AnimalType randomSpecies = (AnimalType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(AnimalType)).Length);
            mockAnimal.animalName = randomName;
            mockAnimal.species = randomSpecies;

            int randomExpiration = currentDayTracker + UnityEngine.Random.Range(1, 4); 
            
            activeRequests.Add(new RescueRequest(mockAnimal, randomExpiration));
        }
    }

    public bool AcceptRescue(RescueRequest request)
    {
        if (!mockHasCorralSpace) return false;

        activeRequests.Remove(request);
        rescueBadge.UpdateBadge(activeRequests.Count);
        OnRequestsUpdated?.Invoke();

        OnRescueAccepted?.Invoke(request.animalData);
        return true;
    }

    public void RejectRescue(RescueRequest request)
    {
        activeRequests.Remove(request);
        rescueBadge.UpdateBadge(activeRequests.Count);
        OnRequestsUpdated?.Invoke();
    }
}