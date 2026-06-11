using UnityEngine;

[System.Serializable]
public class AnimalData
{
    public string animalName;
    public AnimalType species;
    public int variantIndex = -1;
    public float hunger = 100f;
    public float thirst = 100f;
    public float affection = 50f;
    public float health = 100f;
}