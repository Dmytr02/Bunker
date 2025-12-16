using TMPro;
using UnityEngine;

public static class ExternalMethods
{
    public static int GetWordIndexFromCharacter(this TMP_Text textComponent, int charIndex) {
        TMP_TextInfo textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.wordCount; i++) {
            TMP_WordInfo wInfo = textInfo.wordInfo[i];
            // Проверяем, попадает ли индекс символа в диапазон слова
            if (charIndex >= wInfo.firstCharacterIndex && charIndex <= wInfo.lastCharacterIndex) {
                return i; // Возвращаем индекс слова
            }
        }
        return -1;
    }
}
