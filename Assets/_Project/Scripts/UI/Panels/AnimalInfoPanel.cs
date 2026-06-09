using UnityEngine;

/// <summary>
/// Panel de información de un animal. Ejemplo de BasePanel con datos de entrada.
/// </summary>
public class AnimalInfoPanel : BasePanel
{
    // Aquí van tus referencias a TextMeshPro, imágenes, etc.
    // [SerializeField] private TextMeshProUGUI nameText;

    // pausesTime = true por defecto (ver en el Inspector para cambiarlo)

    /// <summary>
    /// Abre el panel mostrando la información del animal recibido.
    /// </summary>
    public void Open(Animal animal)
    {
        UpdateInfo(animal); // datos primero
        base.Open();        // activar después
    }

    public void UpdateInfo(Animal animal)
    {
        if (animal == null) return;

        // nameText.text = animal.AnimalName;
        // Completar con tus campos de UI
    }
}