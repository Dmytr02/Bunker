using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] int sceneIndex;
    
    AsyncOperation async;
    
    public void loadScene()
    {
        async = SceneManager.LoadSceneAsync(sceneIndex);
    }

    private void Update()
    {
        if (text != null && async != null) text.text = ((async.progress/0.9f) * 99).ToString("F0") + "%";
    }
}
