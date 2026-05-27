using System;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

    private static readonly Dictionary<KeyCode, char> SpecialKeys = new Dictionary<KeyCode, char>
    {
        // Цифры основной клавиатуры
        { KeyCode.Alpha0, '0' }, { KeyCode.Alpha1, '1' }, { KeyCode.Alpha2, '2' },
        { KeyCode.Alpha3, '3' }, { KeyCode.Alpha4, '4' }, { KeyCode.Alpha5, '5' },
        { KeyCode.Alpha6, '6' }, { KeyCode.Alpha7, '7' }, { KeyCode.Alpha8, '8' }, { KeyCode.Alpha9, '9' },
        
        // Цифры на Numpad
        { KeyCode.Keypad0, '0' }, { KeyCode.Keypad1, '1' }, { KeyCode.Keypad2, '2' },
        { KeyCode.Keypad3, '3' }, { KeyCode.Keypad4, '4' }, { KeyCode.Keypad5, '5' },
        { KeyCode.Keypad6, '6' }, { KeyCode.Keypad7, '7' }, { KeyCode.Keypad8, '8' }, { KeyCode.Keypad9, '9' },
        
        // Знаки препинания и математические символы
        { KeyCode.KeypadDivide, '/' }, { KeyCode.KeypadMultiply, '*' }, 
        { KeyCode.KeypadMinus, '-' }, { KeyCode.KeypadPlus, '+' }, { KeyCode.KeypadPeriod, '.' },
        { KeyCode.Space, ' ' }, { KeyCode.Minus, '-' }, { KeyCode.Equals, '=' },
        { KeyCode.LeftBracket, '[' }, { KeyCode.RightBracket, ']' }, { KeyCode.Semicolon, ';' },
        { KeyCode.Quote, '\'' }, { KeyCode.Comma, ',' }, { KeyCode.Period, '.' }, 
        { KeyCode.Slash, '/' }, { KeyCode.Backslash, '\\' }, { KeyCode.BackQuote, '`' }
    };
    
    
    public static string KeyCodeString(this KeyCode keyCode)
    {
        if (SpecialKeys.TryGetValue(keyCode, out char specialChar))
        {
            return specialChar.ToString();
        }
        
        return keyCode.ToString();
    }
    
    private static  Dictionary<object, string> stats = new()
    {
        {Professions.Doctor, "Doctor"},
        {Professions.engineer, "Engineer"},
        {Professions.scientist, "Scientist"},
        {Professions.biologistChemist, "Biologist Chemist"},
        {Professions.psychologist, "Psychologist"},
        {Professions.Farmer, "Farmer"},
        {Professions.Soldier, "Soldier"},
        {Professions.Electrician, "Electrician"},
        {Professions.RescueWorker, "Rescue Worker"},
        {Professions.Journalist, "Journalist"},
        {Professions.Teacher, "Teacher"},
        {Professions.SocialWorker, "Social Worker"},
        {Professions.Actor, "Actor"},
        {Professions.Artist, "Artist"},
        {Professions.Student, "Student"},
        {Healthe.excellent, "Excellent"},
        {Healthe.average, "Average"},
        {Healthe.poor, "Poor"},
        {Healthe.critical, "Critical"},
        {Healthe.withdrawal, "Withdrawal"},
        {Healthe.psychosis, "Psychosis"},
        {Healthe.colorBlindness, "Color Blindness"},
        {Phobias.Claustrophobia, "Claustrophobia"},
        {Phobias.FearOfBlood, "Fear Of Blood"},
        {Phobias.FearOfTheDark, "Fear Of The Dark"},
        {Phobias.Anxiety, "Anxiety"},
        {Phobias.FearOfPublicSpeaking, "Fear Of Public Speaking"},
        {Phobias.NoPhobias, "No Phobias"},
        {Hobby.Fishing_Hunting, "Fishing & Hunting"},
        {Hobby.Drawing, "Drawing"},
        {Hobby.Chemistry, "Chemistry"},
        {Hobby.Writing, "Writing"},
        {Hobby.Fitness, "Fitness"},
        {Hobby.Music, "Music"},
        {Hobby.Knitting, "Knitting"},
        {Hobby.ComputerGames, "Computer Games"},
        {Hobby.NoHobbies, "No Hobbies"},
        {Personality.Leader, "Leader"},
        {Personality.Logical, "Logical"},
        {Personality.Stress_resistant, "Stress Resistant"},
        {Personality.Communicator, "Communicator"},
        {Personality.Rational, "Rational"},
        {Personality.Reliable, "Reliable"},
        {Personality.Adaptable, "Adaptable"},
        {Personality.Observant, "Observant"},
        {Personality.Panicker, "Panicker"},
        {Personality.Unstable, "Unstable"},
        {Personality.Egoist, "Egoist"},
        {Personality.Impulsive, "Impulsive"},
        {Personality.Withdrawn, "Withdrawn"}
    };

    public static string StatToString(this object stat)
    {
        return (stats.ContainsKey(stat) ? stats[stat] : stat).ToString();
    }
    
    public static object CastToType(this object value, Type targetType)
    {
        if (targetType == null) throw new ArgumentNullException(nameof(targetType));

        // 1. Obsługa wartości null (zwraca null lub domyślną wartość dla structów)
        if (value == null || value == DBNull.Value)
        {
            return targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null
                ? Activator.CreateInstance(targetType)
                : null;
        }

        // 2. Obsługa typów Nullable (wyciągamy typ bazowy, np. z int? robimy int)
        Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        Type valueType = value.GetType();

        // 3. Jeśli wartość jest już właściwego typu, po prostu ją zwróć
        if (underlyingType.IsAssignableFrom(valueType))
        {
            return value;
        }

        // 4. SPECJALNA OBSŁUGA ENUMÓW (To, czego brakowało w Convert.ChangeType)
        if (underlyingType.IsEnum)
        {
            if (value is string stringValue)
            {
                // Jeśli to string (np. "1" lub "Active"), parsujemy tekstowo
                return Enum.Parse(underlyingType, stringValue, ignoreCase: true);
            }
            // Jeśli to liczba (int, byte itp.), rzutujemy ją bezpośrednio na enum
            return Enum.ToObject(underlyingType, value);
        }

        // 5. Obsługa konwersji ze stringa za pomocą TypeDescriptor (dla niestandardowych klas/struktur)
        if (value is string str)
        {
            var converter = TypeDescriptor.GetConverter(underlyingType);
            if (converter != null && converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFrom(str);
            }
        }

        // 6. Standardowa konwersja (dla int, string, double, bool, DateTime itp.)
        try
        {
            return Convert.ChangeType(value, underlyingType);
        }
        catch
        {
            // Ostateczność: Próba wymuszenia rzutowania referencyjnego (jak tradycyjne rzutowanie)
            return value; 
        }
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
