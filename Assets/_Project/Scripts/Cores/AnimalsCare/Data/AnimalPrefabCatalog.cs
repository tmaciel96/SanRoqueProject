using UnityEngine;

[CreateAssetMenu(fileName = "AnimalPrefabCatalog", menuName = "SanRoque/Animal Prefab Catalog")]
public class AnimalPrefabCatalog : ScriptableObject
{
    [SerializeField] private GameObject dogPrefab;
    [SerializeField] private GameObject catPrefab;

    public GameObject GetPrefab(AnimalType species)
    {
        switch (species)
        {
            case AnimalType.Cat:
                return catPrefab != null ? catPrefab : dogPrefab;
            case AnimalType.Dog:
            default:
                return dogPrefab;
        }
    }

    public int GetVariantCount(AnimalType species)
    {
        GameObject prefab = GetPrefab(species);
        if (prefab == null) return 1;

        Animal animal = prefab.GetComponent<Animal>();
        return animal != null ? animal.VariantCount : 1;
    }
}
