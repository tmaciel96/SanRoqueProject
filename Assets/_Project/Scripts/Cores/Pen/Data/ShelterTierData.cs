using UnityEngine;

[System.Serializable]
public class ShelterTierData
{
    public string label;        // nombre descriptivo, ej: "Corral básico"
    public int unlockCost;      // costo para desbloquear este corral
    public int animalCapacity;  // cuántos animales caben en este corral

    // TODO: cuando haya mejoras de nivel, agregar:
    // public ShelterLevelData[] levels;
}