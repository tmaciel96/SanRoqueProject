using UnityEngine;

public class DrainableIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconRenderer;

    [Range(0f, 1f)]
    public float fillAmount = 1f;

    private Material materialInstance;

    [Header("Pulse")]
    [SerializeField] private float pulseThreshold = 0.1f;
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float pulseAmount = 0.15f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;

        materialInstance = iconRenderer.material =
            new Material(iconRenderer.sharedMaterial);
    }

    private void Update()
    {
        materialInstance.SetFloat("_FillAmount", fillAmount);

        HandlePulse();
    }

    public void SetFill(float value)
    {
        fillAmount = Mathf.Clamp01(value);
    }

    private void HandlePulse()
    {
        if (fillAmount > pulseThreshold)
        {
            transform.localScale = originalScale;
            return;
        }

        float scaleOffset =
            Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

        transform.localScale =
            originalScale * (1f + scaleOffset);
    }
}