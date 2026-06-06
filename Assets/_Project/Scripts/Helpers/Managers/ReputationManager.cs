using UnityEngine;
using System;
using System.Collections;

public class ReputationManager : MonoBehaviour
{
    public static ReputationManager Instance { get; private set; }

    [Header("Configuración de niveles")]
    [SerializeField] private int[] levelThresholds = { 0, 100, 250, 450, 700 };

    [Header("Donaciones")]
    [SerializeField] private int donationMinAmount = 50;
    [SerializeField] private int donationMaxAmount = 200;
    [SerializeField] private float donationCooldown = 30f;

    public int CurrentPoints { get; private set; }
    public int CurrentLevel { get; private set; } = 1;

    public event Action<int> OnPointsChanged;
    public event Action<int> OnLevelUp;
    public event Action<int> OnDonationReceived;

    private Coroutine _donationCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddPoints(int points)
    {
        CurrentPoints += points;
        OnPointsChanged?.Invoke(CurrentPoints);
        CheckLevelUp();
    }

    public void ResetDay()
    {
        CurrentPoints = 0;
        OnPointsChanged?.Invoke(CurrentPoints);
    }

    private void CheckLevelUp()
    {
        for (int i = levelThresholds.Length - 1; i >= 0; i--)
        {
            if (CurrentPoints >= levelThresholds[i] && CurrentLevel < i + 1)
            {
                CurrentLevel = i + 1;
                OnLevelUp?.Invoke(CurrentLevel);

                if (_donationCoroutine != null) StopCoroutine(_donationCoroutine);
                _donationCoroutine = StartCoroutine(DonationLoop());
                break;
            }
        }
    }

    private IEnumerator DonationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(donationCooldown);
            int amount = UnityEngine.Random.Range(donationMinAmount, donationMaxAmount);
            OnDonationReceived?.Invoke(amount);
        }
    }
}