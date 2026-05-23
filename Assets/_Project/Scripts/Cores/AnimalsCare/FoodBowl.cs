using UnityEngine;

public class BowlReceiver : MonoBehaviour
{
    [SerializeField] private Sprite emptyBowl;

    [SerializeField] private Sprite fullBowl;

    [SerializeField] AnimalPen animalPen;

    private SpriteRenderer spriteRenderer;

    private bool isFull = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        EmptyBowl();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DraggableItem draggable = other.GetComponent<DraggableItem>();

        if (draggable != null && !isFull) 
        {
            FillBowl();        
        }
    }

    private void FillBowl()
    {
        isFull = true;

        spriteRenderer.sprite = fullBowl;

        Debug.Log("Está lleno");

        //animalPen.FeedAnimal(20f);
    }

    private void EmptyBowl()
    {
        isFull = false;

        spriteRenderer.sprite = emptyBowl;

        Debug.Log("Bowl vacío");
    }

    public bool IsFull => IsFull;
}
