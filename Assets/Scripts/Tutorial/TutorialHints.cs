using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialHints : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] Animator animator;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject book;
    [SerializeField] GameObject Notepad;
    [SerializeField] CanvasGroup video;
    
    public bool triger = false;
    public bool triger2 = false;

    public bool _triger
    {
        get => triger; 
        set=>triger=value;
    }
    void Start()
    {
        StartCoroutine(Corutine());
    }

    IEnumerator Corutine()
    {
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
        
        Notepad.SetActive(true);
        animator.SetTrigger("triger");
        
        yield return new WaitUntil(() => triger);
        triger = false;
        
        animator.SetTrigger("triger2");
        
        yield return new WaitForSeconds(1);
        
        book.gameObject.SetActive(true);
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

        material.SetFloat("_alpha", 0.03f);
        
        yield return new WaitUntil(() => triger);
        
        triger = false;
        
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
        
        yield return new WaitUntil(() => triger2);
        triger2 = false;
        
        animator.SetTrigger("triger3");
        
        yield return new WaitUntil(() => triger2);
        
        animator.SetTrigger("triger4");
    }
}
