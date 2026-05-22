using TMPro.EditorUtilities;
using UnityEngine;

public class AnimalInteraction : MonoBehaviour
{
    private Animal animal;

    private void Awake()
    {
        animal = GetComponent<Animal>();
    }

    private void OnMouseDown()
    {
        Debug.Log("Está haciendo click");
        UIManager.Instance.ShowAnimalPanel(animal);
    }
}
