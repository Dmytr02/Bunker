using UnityEngine;
using UnityEngine.UI;

public class ImageColorChanger : MonoBehaviour
{
    [SerializeField] Image image;

    public void changeColor(string hexColor)
    {
        if(ColorUtility.TryParseHtmlString(hexColor, out Color myColor)) image.color = myColor;
    }
}
