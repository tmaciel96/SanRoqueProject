using System;
using UnityEngine;

public static class AnimalGenerator
{
    private static readonly string[] DogNames =
    {
        "Bondiola", "Feca", "Pocho", "Yuyo", "Negro",
        "Bala", "Chispa", "Coco", "Pipa",
        "Bash", "Miga", "Pixel", "Astro", "Pampa"
    };

    private static readonly string[] CatNames =
    {
        "Michi", "Luna", "Simba", "Nala", "Garfield",
        "Salem", "Felix", "Whiskers", "Sombra", "Cleo",
        "Milo", "Toby", "Zelda", "Oreo", "Pumpkin"
    };

    /// <summary>
    /// Genera la data pura de un animal de forma aleatoria utilizando el catálogo para calcular las variantes.
    /// </summary>
    public static AnimalData GenerateRandomAnimal(AnimalPrefabCatalog catalog)
    {
        if (catalog == null)
        {
            Debug.LogError("[AnimalGenerator] El catálogo provisto es nulo. No se pueden calcular las variantes.");
            return null;
        }

        // Seleccionar especie al azar basándose en el Enum
        var species = (AnimalType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(AnimalType)).Length);

        // Seleccionar nombre al azar según la especie
        var name = species == AnimalType.Dog
            ? DogNames[UnityEngine.Random.Range(0, DogNames.Length)]
            : CatNames[UnityEngine.Random.Range(0, CatNames.Length)];

        // Obtener el límite de variantes directamente desde tu catálogo
        int totalVariants = catalog.GetVariantCount(species);
        int randomVariantIndex = UnityEngine.Random.Range(0, totalVariants);

        // Crear y rellenar el contenedor de datos pura
        var animalData = new AnimalData
        {
            animalName   = name,
            species      = species,
            variantIndex = randomVariantIndex,
            hunger       = UnityEngine.Random.Range(50f, 100f),
            thirst       = UnityEngine.Random.Range(50f, 100f),
            affection    = UnityEngine.Random.Range(50f, 80f),
            health       = UnityEngine.Random.Range(50f, 80f)
        };

        // Retornamos el objeto con toda su data cargada
        return animalData;
    }
}