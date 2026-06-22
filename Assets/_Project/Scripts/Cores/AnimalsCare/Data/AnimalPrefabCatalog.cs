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
                return catPrefab;
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

        if(animal == null)
        {
            Debug.LogError($"[AnimalPrefabCatalog] El prefab para {species} no tiene un componente Animal. Asegurate de que el prefab tenga el script Animal asignado.");
            return 1;
        }

        return animal.VariantCount;
    }
}
