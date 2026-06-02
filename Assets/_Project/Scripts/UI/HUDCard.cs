using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDCard : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] protected Image icon;
    [SerializeField] protected TextMeshProUGUI labelText;
    [SerializeField] protected TextMeshProUGUI valueText;

    [Header("Estilo")]
    [SerializeField] protected Color labelColor = Color.white;
    [SerializeField] protected Color valueColor = Color.white;

    protected virtual void Awake()
    {
        labelText.color = labelColor;
        valueText.color = valueColor;
    }

    public void SetLabel(string text) => labelText.text = text;
    public void SetValue(string text) => valueText.text = text;
    public void SetIcon(Sprite sprite) => icon.sprite = sprite;
}