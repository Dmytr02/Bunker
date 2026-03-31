using UnityEngine;
using TMPro;

[ExecuteInEditMode] // Чтобы работало прямо в редакторе
public class TMPBackgroundAdjuster : MonoBehaviour
{
    public Transform background; // Ссылка на Plane или Quad
    public Vector2 padding = new Vector2(0.5f, 0.3f); // Отступы в юнитах

    private TMP_Text _textMesh;

    void Awake()
    {
        _textMesh = GetComponent<TMP_Text>();
    }

    void LateUpdate()
    {
        if (background == null) return;

        // Обновляем меш текста, чтобы получить актуальные границы
        _textMesh.ForceMeshUpdate();
        Bounds textBounds = _textMesh.textBounds;

        // 1. Рассчитываем новый масштаб подложки
        // Если используете Quad, его базовый размер 1x1 юнит.
        // Если используете Plane, его размер 10x10 (нужно делить на 10).
        float newWidth = textBounds.size.x + padding.x;
        float newHeight = textBounds.size.y + padding.y;

        background.localScale = new Vector3(newWidth, newHeight, 1f);

        // 2. Центрируем подложку
        // textBounds.center возвращает локальную позицию центра текста
        background.localPosition = new Vector3(textBounds.center.x, textBounds.center.y, 0.01f); 
        // 0.01f — небольшой сдвиг по Z, чтобы текст не "мерцал" внутри подложки (Z-fighting)
    }
}