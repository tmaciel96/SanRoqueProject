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
    [SerializeField] private int rewardPerRescue = 100;

    [Header("UI Component")]
    [SerializeField] private NotificationBadge rescueBadge;

    private List<RescueRequest> activeRequests = new List<RescueRequest>();

    private readonly string[] mockNames = { "Bondiola", "Feca", "Pocho", "Yuyo", "Negro", "Michi", "Bala", "Chispa", "Coco", "Pipa", "Bash", "Miga", "Pixel", "Astro", "Pampa"};

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

            int randomDaysRemaining = UnityEngine.Random.Range(1, 4);            
            activeRequests.Add(new RescueRequest(mockAnimal, randomDaysRemaining));        }
    }

    public bool AcceptRescue(RescueRequest request)
    {
        if (!mockHasCorralSpace) return false;
        if (CapacityManager.Instance != null && !CapacityManager.Instance.TryAddAnimal()) return false;

        activeRequests.Remove(request);
        rescueBadge.UpdateBadge(activeRequests.Count);
        OnRequestsUpdated?.Invoke();

        if (MoneyManager.Instance != null)
            MoneyManager.Instance.AddMoney(rewardPerRescue);

        if (TaskManager.Instance != null)
            TaskManager.Instance.ReportRescue();
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
