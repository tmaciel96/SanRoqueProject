using UnityEngine;
using UnityEngine.EventSystems;
public class DraggableItem2 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

        
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin dragging");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End dragging");
    }

    
}
