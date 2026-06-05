using UnityEngine;

public class CareSource : MonoBehaviour
{
    [SerializeField] private GameObject itemPrfb;

    private void OnMouseDown()
    {
        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePosition.z = 0f;

        GameObject portion = Instantiate(
            itemPrfb, 
            mousePosition, 
            Quaternion.identity
        );

        DraggableItem draggableItem =
            portion.GetComponent<DraggableItem>();

        if (draggableItem != null) draggableItem.BeginDrag();
    }

    
}
