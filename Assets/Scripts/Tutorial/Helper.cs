using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Helper : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Animator animator;

    private List<string> _phrases;
    private int _currentPhase;

    public void SetPhrases(List<string> phrases)
    {
        _phrases = phrases;
        gameObject.SetActive(true);
        _currentPhase = 0;
        animator.SetBool("isShowen", true);
        text.text = _phrases[_currentPhase];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            NextPhrase();
        }
    }

    private void NextPhrase()
    {
        _currentPhase++;
        if (_currentPhase >= _phrases.Count)
        {
            Hide();
            return;
        }
        text.text = _phrases[_currentPhase];
    }

    public void Hide()
    {
        //gameObject.SetActive(false);
        animator.SetBool("isShowen", false);
    }
}
