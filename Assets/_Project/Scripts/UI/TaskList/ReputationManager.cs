using System;
using System.Collections;
using UnityEngine;

public class ReputationManager : MonoBehaviour
{
    public static ReputationManager Instance { get; private set; }

    [Header("Configuración de niveles")]
    [SerializeField] private int[] levelThresholds = { 0, 100, 250, 450, 700 };

    [Header("Donaciones")]
    [SerializeField] private int donationMinAmount = 50;
    [SerializeField] private int donationMaxAmount = 200;
    [SerializeField] private float donationCooldown = 30f; // segundos entre donaciones

    public int CurrentPoints { get; private set; }
    public int CurrentLevel { get; private set; } = 1;

    public event Action<int> OnPointsChanged;   // parámetro: puntos actuales
    public event Action<int> OnLevelUp;          // parámetro: nuevo nivel
    public event Action<int> OnDonationReceived; // parámetro: monto

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

    private void CheckLevelUp()
    {
        for (int i = levelThresholds.Length - 1; i >= 0; i--)
        {
            if (CurrentPoints >= levelThresholds[i] && CurrentLevel < i + 1)
            {
                CurrentLevel = i + 1;
                OnLevelUp?.Invoke(CurrentLevel);
                StartCoroutine(DonationLoop());
                break;
            }
        }
    }

    private IEnumerator DonationLoop()
    {
        yield return new WaitForSeconds(donationCooldown);
        int amount = UnityEngine.Random.Range(donationMinAmount, donationMaxAmount);
        OnDonationReceived?.Invoke(amount);
    }
}