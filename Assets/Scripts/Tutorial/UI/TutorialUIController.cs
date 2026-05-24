using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TutorialUIController : StateMachine.StateMachine<TutorialUIController>
{
    public GameObject PousePanel;
    public Settings SettingsPanel;
    public EventTrigger SettingsButton;
    public TutorialCommandManager commandManager;
    public Animator loadingScreenAnimator;
    public SavedKey chatKey;
    
    public static TutorialUIController instance;

    private void Awake()
    {
        if(instance) Destroy(this);
        else instance = this;
    }

    private void Start()
    {
        chatKey.Init();
        Begin(new TutorialUIGameState(this));
    }
    
    public void MainMenu()
    {
        loadingScreenAnimator.SetTrigger("loadScene"); 
    }
}
