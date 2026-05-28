using UnityEngine;

public class BubbleTest : MonoBehaviour
{
    public NeedBubble bubble;

    [Header("Auto-vaciar")]
    public bool autoVaciar = false;
    public float velocidad = 0.1f; // unidades por segundo

    [Header("Control manual")]
    [Range(0f, 1f)] public float fillManual = 1f;

    private float _fillActual = 1f;

    void Update()
    {
        if (autoVaciar)
        {
            _fillActual -= velocidad * Time.deltaTime;
            _fillActual = Mathf.Clamp01(_fillActual);
            bubble.SetFill(_fillActual);
        }
        else
        {
            // control manual desde el Inspector en Play Mode
            bubble.SetFill(fillManual);
        }

        // R para resetear
        if (Input.GetKeyDown(KeyCode.R))
        {
            _fillActual = 1f;
            fillManual = 1f;
        }
    }
}