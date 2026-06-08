using UnityEngine;

[CreateAssetMenu(fileName = "ShelterDatabase", menuName = "SanRoque/Shelter Database")]
public class ShelterDatabase : ScriptableObject
{
    public ShelterTierData[] tiers;

    /// <summary>
    /// Retorna el tier correspondiente al índice del corral.
    /// Si el índice supera la cantidad de tiers definidos, retorna el último.
    /// </summary>
    public ShelterTierData GetTier(int index)
    {
        if (tiers == null || tiers.Length == 0)
        {
            Debug.LogWarning("ShelterDatabase: no hay tiers definidos.");
            return null;
        }

        int clampedIndex = Mathf.Clamp(index, 0, tiers.Length - 1);
        return tiers[clampedIndex];
    }

    public int TierCount => tiers?.Length ?? 0;
}