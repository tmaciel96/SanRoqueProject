using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HUDCard : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] protected Image icon;
    [SerializeField] protected TextMeshProUGUI labelText;
    [SerializeField] protected TextMeshProUGUI valueText;

    [Header("Estilo")]
    [SerializeField] protected Color labelColor = new Color(1f, 1f, 1f, 0.6f);
    [SerializeField] protected Color valueColor = Color.white;

    // Para animación de cambio de valor
    private Coroutine _flashCoroutine;
    [SerializeField] private Color flashPositive = new Color(0.4f, 1f, 0.4f);
    [SerializeField] private Color flashNegative = new Color(1f, 0.4f, 0.4f);

    protected virtual void Awake()
    {
        labelText.color = labelColor;
        valueText.color = valueColor;
    }

    public void SetLabel(string text) => labelText.text = text;

    public void SetValue(string text) => valueText.text = text;


    public void SetIcon(Sprite sprite) => icon.sprite = sprite;

    // En HUDCard.cs — reemplazá los dos métodos viejos de flash
    protected void FlashValue(Color flashColor)
    {
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashRoutine(flashColor));
    }

    private IEnumerator FlashRoutine(Color flashColor)
    {
        valueText.color = flashColor;
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            valueText.color = Color.Lerp(flashColor, valueColor, t / 0.4f);
            yield return null;
        }
        valueText.color = valueColor;
    }
}