using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIController : StateMachine.StateMachine<UIController>
{
    public GameObject PousePanel;
    public Settings SettingsPanel;
    public EventTrigger SettingsButton;
    public CommandManager commandManager;
    public Animator loadingScreenAnimator;
    public SavedKey chatKey;
    public SavedKey cameraKey;
    public bool isFollowCursore = true;
    
    public static UIController instance;

    private void Awake()
    {
        if(instance) Destroy(this);
        else instance = this;
    }

    private void Start()
    {
        chatKey.Init();
        cameraKey.Init();
        Begin(new UIGameState(this));
    }
    
    public void MainMenu()
    {
        PhotonNetwork.LeaveRoom();
        loadingScreenAnimator.SetTrigger("loadScene"); 
    }
}
