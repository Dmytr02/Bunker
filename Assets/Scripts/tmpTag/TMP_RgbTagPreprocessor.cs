using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "RGB Tag Preprocessor", menuName = "TextMeshPro/Custom Preprocessors/Ultimate RGB Tag")]
public class TMP_RgbTagPreprocessor : ScriptableObject, ITextPreprocessor
{
    private static readonly Regex AlphaRegex = new Regex(@"<alpha=#([A-Fa-f0-9]{2})>", RegexOptions.Compiled);
    private static readonly Regex RgbOpenRegex = new Regex(@"<rgb=#([A-Fa-f0-9]{6})>", RegexOptions.Compiled);
    private static readonly Regex RgbCloseRegex = new Regex(@"</rgb>", RegexOptions.Compiled);

    public string PreprocessText(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        // 1. Сначала обрабатываем открывающие теги <rgb=#RRGGBB>
        string processed = RgbOpenRegex.Replace(text, match =>
        {
            string currentGlobalAlpha = GetActiveAlpha(text, match.Index);
            string hexRgb = match.Groups[1].Value;

            // Превращаем в 8-значный цвет с учетом текущей альфы
            return $"<color=#{hexRgb}{currentGlobalAlpha}>";
        });

        // 2. Теперь обрабатываем закрывающие теги </rgb>
        // Вместо простого </color>, мы закрываем цвет и СРАЗУ ЖЕ восстанавливаем альфу,
        // которая должна быть активна в этой части строки.
        processed = RgbCloseRegex.Replace(processed, match =>
        {
            string currentGlobalAlpha = GetActiveAlpha(processed, match.Index);
            
            // Закрываем цвет и тут же накатываем обратно тег alpha, чтобы текст дальше не ломался
            return $"</color><alpha=#{currentGlobalAlpha}>";
        });

        return processed;
    }

    // Вспомогательный метод: ищет, какая альфа сейчас активна в строке перед указанным индексом
    private string GetActiveAlpha(string currentText, int searchIndex)
    {
        string textBefore = currentText.Substring(0, searchIndex);
        var alphaMatches = AlphaRegex.Matches(textBefore);

        if (alphaMatches.Count > 0)
        {
            // Возвращаем значение последнего найденного тега <alpha>
            return alphaMatches[alphaMatches.Count - 1].Groups[1].Value;
        }

        // Если тегов <alpha> до этого места не было, возвращаем FF (100% видимость)
        return "FF";
    }
}
