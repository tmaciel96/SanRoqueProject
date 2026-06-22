using System;
using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    [SerializeField] private CareType careType;
    
    private float pettingTimer;
    [SerializeField] private float pettingInterval = 0.9f;

    private bool isDragging = false;

    private Vector3 offset;

    private Animal hoveredAnimal;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning($"{name} no encontró Animator.");
        }
    }

    private void OnMouseDown()
    {
        BeginDrag();
    }

    private void OnMouseUp()
{
    isDragging = false;

    if (careType == CareType.Petting && animator != null)
        animator.SetBool("IsPetting", false);

    if (careType != CareType.Petting && hoveredAnimal != null)
    {
        hoveredAnimal.ApplyCare(careType);
        ConsumeInventoryAndReport(careType);
        Destroy(gameObject);
        return;
    }

    Destroy(gameObject);
}

private void ConsumeInventoryAndReport(CareType care)
{
    switch (care)
    {
        case CareType.Food:
            InventoryManager.Instance.ConsumeItem(InventoryItemType.Food, 1);
            break;
        case CareType.Medicine:
            InventoryManager.Instance.ConsumeItem(InventoryItemType.Medicine, 1);
            break;
        // Water y Petting no consumen inventario
    }

    TaskManager.Instance?.ReportCare(care);
}

    void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;

            if(Input.GetMouseButtonDown(1))
            {
                Destroy( gameObject );
                return;
            }
        }

        if (isDragging && careType == CareType.Petting && hoveredAnimal != null)
        {
            pettingTimer += Time.deltaTime;

            if (pettingTimer > pettingInterval)
            {
                hoveredAnimal.ApplyCare(CareType.Petting);
                TaskManager.Instance?.ReportCare(CareType.Petting);
                pettingTimer = 0f;
            }
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

            if (careType == CareType.Petting && animator != null)
            {
                animator.SetBool("isPetting", true);
            }

            Debug.Log($"Entr� en contacto con el {animal.AnimalName}");

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Animal animal = other.GetComponent<Animal>();

        if (animal == hoveredAnimal)
        {
            hoveredAnimal = null;

            if (careType == CareType.Petting && animator != null)
            {
                animator.SetBool("isPetting", false);
            }
        }
    }

    public void BeginDrag()
    {
        isDragging = true;

        offset = transform.position - GetMouseWorldPosition();

        
    }

    private void reportAppliedCare(CareType careType){
        if (TaskManager.Instance == null)
            Debug.LogWarning("[DraggableItem] No se encontro la referencia a TaskManager");

        TaskManager.Instance.ReportCare(careType);
    }
}
