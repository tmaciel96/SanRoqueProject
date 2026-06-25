using System.Collections;
using UnityEngine;

public class AdoptionBubble : MonoBehaviour
{
    [SerializeField] private Animal animal;

    [Header("Pop Animation")]
    [SerializeField] private float popDuration = 0.2f;
    [SerializeField] private float popScale = 1.2f;

    [Header("Pulse")]
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseAmount = 0.08f;

    private Vector3 originalScale;
    private bool canPulse;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(PopRoutine());
    }

    private void Update()
    {
        if (!canPulse)
            return;

        float scale =
            1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

        transform.localScale = originalScale * scale;
    }

    private IEnumerator PopRoutine()
    {
        canPulse = false;

        transform.localScale = Vector3.zero;

        float timer = 0f;

        while (timer < popDuration)
        {
            timer += Time.deltaTime;

            float t = timer / popDuration;

            // Overshoot
            float scale = Mathf.Lerp(0f, popScale, t);

            transform.localScale = originalScale * scale;

            yield return null;
        }

        timer = 0f;

        while (timer < popDuration * 0.5f)
        {
            timer += Time.deltaTime;

            float t = timer / (popDuration * 0.5f);

            float scale = Mathf.Lerp(popScale, 1f, t);

            transform.localScale = originalScale * scale;

            yield return null;
        }

        transform.localScale = originalScale;

        canPulse = true;
    }

    private void OnMouseDown()
    {
        if (animal == null) return;

        animal.Adoption();

        Debug.Log($"Animal {animal.name} adopted!");

        gameObject.SetActive(false);
    }
}