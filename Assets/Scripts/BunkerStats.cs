
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BunkerStats : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField, TextArea(5, 100)] private string stats;
    [SerializeField] private UnityEvent OnHide;

    void Start()
    {
        StartCoroutine(Corutine());
    }

    IEnumerator Corutine()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        text.text = stats;
        yield return null;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        gameObject.SetActive(false);
        OnHide?.Invoke();
    }
}
