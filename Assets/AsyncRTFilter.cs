using UnityEngine;
using UnityEngine.UI;

public class AsyncRTFilter : MonoBehaviour, ICanvasRaycastFilter
{
    [SerializeField] private RenderTexture renderTexture;
    private RectTransform rectTransform;
    private Texture2D tinyTexture; // Временный буфер для 1 пикселя

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Создаем текстуру 1x1 для чтения
        tinyTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
    }

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (renderTexture == null || !renderTexture.IsCreated()) return true;

        // 1. Получаем UV-координаты клика
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out localPoint);
        
        // Перевод в диапазон 0..1
        float u = (localPoint.x - rectTransform.rect.x) / rectTransform.rect.width;
        float v = (localPoint.y - rectTransform.rect.y) / rectTransform.rect.height;
        if (u < 0 || u > 1 || v < 0 || v > 1) return false;
        // 2. Читаем пиксель из RT
        return GetAlphaFromRT(u, v) < 0.1f;
    }

    float GetAlphaFromRT(float u, float v)
    {
        int x = (int)(u * renderTexture.width);
        int y = (int)(v * renderTexture.height);

        // Запоминаем текущий активный RT, чтобы не сломать рендер Unity
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // Копируем только нужный пиксель
        tinyTexture.ReadPixels(new Rect(x, y, 1, 1), 0, 0);
        tinyTexture.Apply();

        RenderTexture.active = previous;

        return tinyTexture.GetPixel(0, 0).a;
    }
}