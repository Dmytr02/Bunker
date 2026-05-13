using TMPro;
using UnityEngine;

public class ApplyRgbTag : MonoBehaviour
{
    public TMP_Text textComponent;
    public TMP_RgbTagPreprocessor rgbPreprocessor;

    void Start()
    {
        if(!textComponent) textComponent=GetComponent<TMP_Text>();
        textComponent.textPreprocessor = rgbPreprocessor;
    }
}