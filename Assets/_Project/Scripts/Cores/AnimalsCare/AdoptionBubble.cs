using UnityEngine;

public class AdoptionBubble : MonoBehaviour
{
    [SerializeField] private Animal animal;

    private void OnMouseDown()
    {
         
        if (animal == null) return;

            animal.Adoption();
    }
}