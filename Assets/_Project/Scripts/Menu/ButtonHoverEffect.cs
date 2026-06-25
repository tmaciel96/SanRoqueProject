using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración de Escala")]
    [SerializeField] private Vector3 targetScale = new Vector3(1.1f, 1.1f, 1.1f);
    [SerializeField] private float speed = 15f; // Velocidad de la transición

    private Vector3 originalScale;

    void Start()
    {
        // Guardamos la escala inicial del botón
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Esto no es estrictamente necesario si usas Corrutinas o iTween, 
        // pero un Lerp en el Update es la forma más directa y limpia sin librerías externas.
    }

    // Se ejecuta cuando el mouse ENTRA al botón
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(targetScale));
    }

    // Se ejecuta cuando el mouse SALE del botón
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale));
    }

    // Corrutina para que el cambio de tamaño sea suave y no un golpe seco
    private System.Collections.IEnumerator ScaleTo(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * speed);
            yield return null;
        }
        transform.localScale = target;
    }
}