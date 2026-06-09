using UnityEngine;

public class SimpleBubble : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer fillRenderer;

    [Range(0f, 1f)]
    public float fillAmount = 1f;

    private Material materialInstance;

    private void Awake()        
    {
        materialInstance =
            fillRenderer.material =
            new Material(fillRenderer.sharedMaterial);
    }

    private void Update()
    {
        materialInstance.SetFloat(
            "_FillAmount",
            fillAmount);
    }
}