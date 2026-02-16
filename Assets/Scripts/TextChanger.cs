using TMPro;
using UnityEngine;

public class TextChanger : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public void ChangeText(string text)
    {
        this.text.text = text;
    }
}
