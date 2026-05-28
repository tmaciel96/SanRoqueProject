using UnityEngine;

public class NeedBubble : MonoBehaviour
{
    [Header("Referencias")]
    public SpriteRenderer fillRenderer;
    public SpriteRenderer iconRenderer;
    public Sprite[] careIcons; // asigná los sprites en el Inspector

    [Header("Colores")]
    public Color colorFull   = new Color(0.30f, 0.69f, 0.31f); // verde
    public Color colorMid    = new Color(1.00f, 0.76f, 0.03f); // amarillo
    public Color colorEmpty  = new Color(0.96f, 0.26f, 0.21f); // rojo

    [Header("Umbrales")]
    [Range(0f, 1f)] public float thresholdMid   = 0.50f;
    [Range(0f, 1f)] public float thresholdEmpty = 0.25f;

    [Range(0f, 1f)] public float fillAmount = 1f;

    private Material _mat;

    void Awake()
    {
        // instanciamos el material para que cada burbuja sea independiente
        _mat = fillRenderer.material = new Material(fillRenderer.sharedMaterial);
    }

    void Update()
    {
        ApplyFill(fillAmount);
    }

    public void SetFill(float value)
    {
        fillAmount = Mathf.Clamp01(value);
    }

    public void SetIcon(int careIndex)
    {
        if (careIndex < careIcons.Length)
            iconRenderer.sprite = careIcons[careIndex];
    }

    void ApplyFill(float t)
    {
        _mat.SetFloat("_FillAmount", t);
        _mat.SetColor("_FillColor", GetFillColor(t));
    }

    Color GetFillColor(float t)
    {
        if (t > thresholdMid)
            return Color.Lerp(colorMid, colorFull, (t - thresholdMid) / (1f - thresholdMid));
        else if (t > thresholdEmpty)
            return Color.Lerp(colorEmpty, colorMid, (t - thresholdEmpty) / (thresholdMid - thresholdEmpty));
        else
            return colorEmpty;
    }
}