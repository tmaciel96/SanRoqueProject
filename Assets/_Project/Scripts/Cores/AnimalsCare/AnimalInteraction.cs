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
        Debug.Log("Est� haciendo click");   
        UIManager2.Instance.ShowAnimalPanel(animal);
    }
}
