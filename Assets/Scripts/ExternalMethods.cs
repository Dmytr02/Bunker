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
    
    // accelerating from zero velocity
    public static float easeInQuad(this float t) => t * t; 
    // decelerating to zero velocity
    public static float easeOutQuad(this float t) => t*(2-t);
    // acceleration until halfway, then deceleration
    public static float easeInOutQuad(this float t) => t < .5 ? 2 * t * t : -1 + (4 - 2 * t) * t;
    // accelerating from zero velocity 
    public static float easeInCubic(this float t) => t * t * t;
    // decelerating to zero velocity 
    public static float easeOutCubic(this float t) => (--t) * t * t + 1;
    // acceleration until halfway, then deceleration 
    public static float easeInOutCubic(this float t) => t < .5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
    // accelerating from zero velocity 
    public static float easeInQuart(this float t) => t * t * t * t;
    // decelerating to zero velocity 
    public static float easeOutQuart(this float t) => 1-(--t)*t*t*t;
    // acceleration until halfway, then deceleration
    public static float easeInOutQuart(this float t) => t < .5 ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;
    // accelerating from zero velocity
    public static float easeInQuint(this float t) => t * t * t * t * t;
    // decelerating to zero velocity
    public static float easeOutQuint(this float t) => 1 + (--t) * t * t * t * t;
    // acceleration until halfway, then deceleration 
    public static float easeInOutQuint(this float t) => t < .5 ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t;
}
