using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialHints : MonoBehaviour
{
    [SerializeField] GameObject book2;
    [SerializeField] GameObject card;
    [SerializeField] GameObject inventoryIcon;
    [SerializeField] TMP_Text hintText;
    [SerializeField] GameObject[] Boty;
    [SerializeField] TutorialHintData[] hintObjects;
    [SerializeField] CanvasGroup darkImage;
    
    
    public bool triger = false;
    public bool _triger2 = false;

    public bool _triger
    {
        get => triger; 
        set=>triger=value;
    }
    public bool triger2
    {
        get => _triger2; 
        set => _triger2=value;
    }

    void Start()
    {
        //StartCoroutine(Corutine());
        darkImage.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
    }

    IEnumerator show(int id, Func<bool> waitFunc = null)
    {
        if (waitFunc == null) waitFunc = () => { return Input.GetKeyDown(KeyCode.Space); };
        hintText.text = hintObjects[id].text;
        darkImage.alpha = 0;
        darkImage.blocksRaycasts = true;
        foreach (var i in hintObjects[id].go)
        {
            if (i != null) i.SetActive(true);
        }

        float t = 0;
        while (t < 0.5f)
        {
            t+=Time.deltaTime;
            darkImage.alpha = Mathf.Lerp(0, 1, t/0.5f);
            yield return null;
        }
        
        darkImage.alpha = 1;
        
        yield return new WaitUntil(waitFunc);
        
        t = 0;
        while (t < 0.5f)
        {
            t+=Time.deltaTime;
            darkImage.alpha = Mathf.Lerp(1, 0, t/0.5f);
            yield return null;
        }
        
        darkImage.alpha = 0;
        darkImage.blocksRaycasts = false;
        foreach (var i in hintObjects[id].go) {
            if(i!=null) i.SetActive(false);
        }
    }

    IEnumerator Corutine()
    {
        yield return show(0);
        //yield return new WaitForSeconds(1);
        yield return show(1, () => { if (triger) { triger = false; return true; } return false; });
        
        foreach (var i in Boty)
        {
            i.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        yield return show(2, () => { if (triger) { triger = false; return true; } return false; });
        yield return new WaitForSeconds(1);
        yield return show(3, () => { if (triger) { triger = false; return true; } return false; });
        yield return new WaitForSeconds(1);
        triger2 = true;
        yield return new WaitUntil(() => triger);
        triger = false;
        inventoryIcon.SetActive(true);
        yield return show(4, () => { if (triger) { triger = false; return true; } return false; });
        yield return new WaitForSeconds(1);
        yield return show(5, () => { if (triger) { triger = false; return true; } return false; });
        yield return new WaitForSeconds(1);
        triger2 = true;
        yield return show(6, () => { if (triger) { triger = false; return true; } return false; });
    }
}
[Serializable]
public class TutorialHintData
{
    [SerializeField] public GameObject[] go;
    [SerializeField, TextArea(4, 100)] public string text;
}
