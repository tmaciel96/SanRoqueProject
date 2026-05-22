using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement")]
    private Camera cam;
    private Vector3 dragOrigin;

    [Header("Background Bounds")]
    [SerializeField] private SpriteRenderer backgroundSprite;
    
    private float minX, maxX;
    private float minY, maxY;

    void Start()
    {
        cam = Camera.main;
        CalculateBounds();
    }

    void CalculateBounds()
    {
        if (backgroundSprite == null)
        {
            Debug.LogError("Please assign the Background SpriteRenderer in the Inspector!");
            return;
        }

        // Get the total boundaries of the background sprite in world units
        Bounds bgBounds = backgroundSprite.bounds;
        float bgMinX = bgBounds.min.x;
        float bgMaxX = bgBounds.max.x;
        float bgMinY = bgBounds.min.y;
        float bgMaxY = bgBounds.max.y;

        // Calculate camera dimensions based on orthographic size and aspect ratio
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        // Clamp limits: The center of the camera cannot go beyond these values
        minX = bgMinX + camHalfWidth;
        maxX = bgMaxX - camHalfWidth;
        minY = bgMinY + camHalfHeight;
        maxY = bgMaxY - camHalfHeight;

        // Guard clause: If the background is smaller than the camera screen
        if (minX > maxX) minX = maxX = bgBounds.center.x;
        if (minY > maxY) minY = maxY = bgBounds.center.y;
    }

    void Update()
    {
        PanCamera();
    }

    void LateUpdate()
    {
        ClampCamera();
    }

    void PanCamera()
    {
        // When the user clicks down, save the initial world position of the mouse
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        // While holding down, calculate the difference vector and move the camera
        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = dragOrigin - currentMousePos;

            // Maintain the camera's original Z position
            difference.z = 0; 

            // Move the camera by shifting its position
            transform.position += difference;
        }
    }

    void ClampCamera()
    {
        // Keep the camera inside the calculated boundaries
        Vector3 clampedPosition = transform.position;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);

        transform.position = clampedPosition;
    }
}
