using TMPro;
using UnityEngine;

public class TMPChangeColor : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public Color defultColor;

    public void SetDefultColor(string hexColor)
    {
        if(ColorUtility.TryParseHtmlString(hexColor, out Color myColor)) defultColor = myColor;
        ResetColor();
    }
    public void SetColor(string hexColor)
    {
        if(ColorUtility.TryParseHtmlString(hexColor, out Color myColor)) text.color = myColor;
    }

    public void ResetColor()
    {
        text.color = defultColor;
    }
}
