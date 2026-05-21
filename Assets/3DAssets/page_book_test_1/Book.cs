using System;
using UnityEngine;

public class Book : MonoBehaviour
{
    Animator animator;
    public int selectedPage = 0;
    public int speed = 10;
    public int speed2 = 10;
    private float _speed;
    private void Start()
    {
        if(animator == null) animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _speed = Mathf.Lerp(_speed, selectedPage, speed * Time.deltaTime);
        animator.SetFloat("Blend", Mathf.Lerp(animator.GetFloat("Blend"), _speed, speed2 * Time.deltaTime));
    }
}
