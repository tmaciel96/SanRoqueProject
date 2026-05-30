using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    [SerializeField] private CareType careType;

    private bool isDragging = false;

    private Vector3 offset;

    private Animal hoveredAnimal;

    private void OnMouseDown()
    {
        isDragging = true;

        offset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseUp() 
    { 
        isDragging = false;
        
        if(hoveredAnimal != null )
        {
            hoveredAnimal.ApplyCare(careType);

            Debug.Log($"Aplicado {careType} a {hoveredAnimal.AnimalName}");
        }

    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }    
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        mousePosition.z = 10f;

        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Animal animal = other.GetComponent<Animal>();

        if (animal != null)
        {
            hoveredAnimal = animal;
            Debug.Log($"Entró en contacto con el {animal.AnimalName}");
        }
    }
}
