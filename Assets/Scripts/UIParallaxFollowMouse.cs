using UnityEngine;

using UnityEngine;

public class UIParallaxFollowMouse : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float parallaxAmount = 20f; // Max movement in pixels
    public float smoothSpeed = 5f;     // Smoothing factor

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        // Normalize mouse position (-1 to 1)
        float x = (mousePos.x / Screen.width - 0.5f) * 2f;
        float y = (mousePos.y / Screen.height - 0.5f) * 2f;

        // Calculate offset
        Vector2 targetOffset = new Vector2(x, y) * parallaxAmount;
        Vector2 targetPosition = originalPosition + targetOffset;

        // Smooth movement
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * smoothSpeed);
    }
}

