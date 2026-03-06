using TMPro;
using UnityEngine;

public class NameState : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private string text = "Name: ";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nameText.text = $"{text} {PlayerPrefs.GetString("name", "Name")}";
    }
}
