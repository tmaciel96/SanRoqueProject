using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    private bool isDragging = false;

    private Vector3 offset;

    private void OnMouseDown()
    {
        isDragging = true;

        offset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseUp() { isDragging = false; }

    // Update is called once per frame
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
}
