using UnityEngine;

/// <summary>
/// Clase base para todos los paneles del juego.
/// Se auto-registra en el UIManager al iniciarse, sin necesidad de asignarlo en el Inspector.
/// </summary>
public abstract class BasePanel : MonoBehaviour
{
    [Header("Panel Config")]
    [Tooltip("Si está activado, abrir este panel cambia el estado del juego a InMenu.")]
    [SerializeField] protected bool pausesTime = true;

    public bool PausesTime => pausesTime;

    protected virtual void Awake()
    {
        // Se registra solo al arrancar la escena
        if (UIManager.Instance != null)
            UIManager.Instance.RegisterAvailablePanel(this);
    }

    protected virtual void OnDestroy()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.UnregisterAvailablePanel(this);
    }

    /// <summary>
    /// Abre el panel. Sobrescribir para agregar lógica específica (siempre llamar base.Open()).
    /// </summary>
    public virtual void Open()
    {
        gameObject.SetActive(true);
        UIManager.Instance.RegisterPanel(this);
    }

    /// <summary>
    /// Cierra el panel. Sobrescribir para agregar lógica específica (siempre llamar base.Close()).
    /// </summary>
    public virtual void Close()
    {
        gameObject.SetActive(false);
        UIManager.Instance.UnregisterPanel(this);
    }
}