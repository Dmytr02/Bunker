using System;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] Animator animator;
    public Observed<int> selectedPage =  new();
    [SerializeField] private int pages;

    public int speed = 10;
    private float last = 0; 
    private float lastUpdate = 0; 
    private void Start()
    {
        selectedPage.Bind((n) => { return n; },(n) => {
            last = animator.GetFloat("Blend");
            lastUpdate = Time.time;
            return (n+pages)%pages;
        });
        if(animator == null) animator = GetComponent<Animator>();
    }

    


    void Update()
    {
        animator.SetFloat("Blend", f(last, selectedPage.Value, Mathf.Clamp01((Time.time-lastUpdate)*speed)));
    }

    float f(float a, float b, float t)
    {
        return Mathf.Lerp(a, b, t.easeOutQuint());
    }
    
    
}
