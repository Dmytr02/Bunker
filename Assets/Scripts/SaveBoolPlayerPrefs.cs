using UnityEngine;

public class SaveBoolPlayerPrefs : MonoBehaviour
{
    [SerializeField] private string name;
    public void SetBoolPlayerPrefs(bool value)
    {
        PlayerPrefs.SetInt(name, value ? 1 : 0);
    }
}
