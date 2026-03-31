using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialHints : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] Animator animator;
    [SerializeField] Animator notepadAnimator;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject book;
    [SerializeField] GameObject book2;
    [SerializeField] GameObject card;
    [SerializeField] GameObject inventoryIcon;
    [SerializeField] CanvasGroup video;
    [SerializeField] TMP_Text hintText;
    
    [SerializeField, TextArea(3, 100)] string[] hints;
    
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

    private void OnEnable()
    {
        material.SetFloat("_alpha", 1);
    }

    private void OnDisable()
    {
        material.SetFloat("_alpha", 1);
    }

    void Start()
    {
        StartCoroutine(Corutine());
    }

    IEnumerator Corutine()
    {
        // кнопка
        hintText.text = hints[0];
        video.alpha = 0;
        video.ignoreParentGroups = true;
        startButton.gameObject.SetActive(true);
        float t = 0;
        while (t < 0.5f)
        {
            t+=Time.deltaTime;
            material.SetFloat("_alpha", Mathf.Lerp(1, 0.03f, t/0.5f));
            video.alpha = Mathf.Lerp(0, 1, t/0.5f);
            yield return null;
        }

        material.SetFloat("_alpha", 0.03f);
        
        yield return new WaitUntil(() => triger);
        triger = false;
        
        // блокнот
        
        
        hintText.text = hints[1];
        t = 0;
        while (t < 0.5f)
        {
            t+=Time.deltaTime;
            material.SetFloat("_alpha", Mathf.Lerp(0.03f, 1, t/0.5f));
            video.alpha = Mathf.Lerp(1, 0, t/0.5f);
            yield return null;
        }
        
        video.ignoreParentGroups = false;
        video.alpha = 1;
        startButton.gameObject.SetActive(false);
        
        material.SetFloat("_alpha", 1);
        
        animator.SetTrigger("triger");
        
        Debug.Log("Trigger finished");
        yield return new WaitUntil(() => triger);
        triger = false;
        Debug.Log("Trigger finished1");
        hintText.text = hints[2];
        
        animator.SetTrigger("triger");
        
        yield return new WaitUntil(() => triger);
        triger = false;
        
        animator.SetTrigger("triger2");
        
        notepadAnimator.SetTrigger("trigger");
        
        yield return new WaitForSeconds(1);
        
        // книга
        
        book.gameObject.SetActive(true);
        video.alpha = 0;
        video.ignoreParentGroups = true;
        hintText.text = hints[3];
        t = 0;
        while (t < 0.5f)
        {
            t+=Time.deltaTime;
            material.SetFloat("_alpha", Mathf.Lerp(1, 0.03f, t/0.5f));
            video.alpha = Mathf.Lerp(0, 1, t/0.5f);
            yield return null;
        }

        material.SetFloat("_alpha", 0.03f);
        
        yield return new WaitUntil(() => triger);
        triger = false;
        triger2 = true;
        
        
        t = 0;
        while (t < 0.5f)
        {
            t+=Time.deltaTime;
            material.SetFloat("_alpha", Mathf.Lerp(0.03f, 1, t/0.5f));
            video.alpha = Mathf.Lerp(1, 0, t/0.5f);
            yield return null;
        }
        video.ignoreParentGroups = false;
        video.alpha = 1;
        book.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(3);
        
        // инвентарь
        
        yield return new WaitUntil(() => _triger2);
        _triger2 = false;
        hintText.text = hints[4];
        
        inventoryIcon.SetActive(true);
        animator.SetTrigger("triger");
        
        yield return new WaitUntil(() => _triger2);
        triger2 = false;
        
        animator.SetTrigger("triger");
        
            // 2 этап(подсвет карты и книги) 
            hintText.text = hints[5];
        
        book2.gameObject.SetActive(true);
        card.gameObject.SetActive(true);
        video.alpha = 0;
        video.ignoreParentGroups = true;
        t = 0;
        while (t < 0.5f)
        {
            t+=Time.deltaTime;
            material.SetFloat("_alpha", Mathf.Lerp(1, 0.03f, t/0.5f));
            video.alpha = Mathf.Lerp(0, 1, t/0.5f);
            yield return null;
        }

        
        yield return new WaitUntil(() => _triger2);
        triger2 = false;
        
        t = 0;
        while (t < 0.5f)
        {
            t+=Time.deltaTime;
            material.SetFloat("_alpha", Mathf.Lerp(0.03f, 1, t/0.5f));
            video.alpha = Mathf.Lerp(1, 0, t/0.5f);
            yield return null;
        }
        video.ignoreParentGroups = false;
        video.alpha = 1;
        book2.gameObject.SetActive(false);
        //card.gameObject.SetActive(false);
        
        
        triger = true;
        
        // голосование
        
        hintText.text = hints[6];
        
        animator.SetTrigger("triger3");
        
        yield return new WaitUntil(() => _triger2);
        
        animator.SetTrigger("triger4");
    }
}
