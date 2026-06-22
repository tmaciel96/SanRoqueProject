using UnityEngine;
using System.Collections;

public class RescueButtonWobble : MonoBehaviour
{
    [SerializeField] private float wobbleAngle = 8f;
    [SerializeField] private float speed = 4f;

    private Coroutine wobbleCoroutine;
    private bool isWobbling;

    public void StartWobble()
    {
        if (isWobbling) return;
        isWobbling = true;
        wobbleCoroutine = StartCoroutine(Wobble());
    }

    public void StopWobble()
    {
        if (!isWobbling) return;
        isWobbling = false;

        if (wobbleCoroutine != null)
            StopCoroutine(wobbleCoroutine);

        transform.localRotation = Quaternion.identity;
    }

    private IEnumerator Wobble()
    {
        while (true)
        {
            float z = Mathf.Sin(Time.time * speed) * wobbleAngle;
            transform.localRotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
    }
}