using UnityEngine;

public class DrainableIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconRenderer;

    [Range(0f, 1f)]
    public float fillAmount = 1f;

    private Material materialInstance;

    private void Awake()
    {
        materialInstance = iconRenderer.material =
            new Material(iconRenderer.sharedMaterial);
    }

    private void Update()
    {
        materialInstance.SetFloat("_FillAmount", fillAmount);
    }

    public void SetFill(float value)
    {
        fillAmount = Mathf.Clamp01(value);
    }
}