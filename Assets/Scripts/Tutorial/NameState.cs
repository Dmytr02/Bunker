using TMPro;
using UnityEngine;

public class NameState : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nameText.text = $"Name: {PlayerPrefs.GetString("name", "Name")}";
    }
}
