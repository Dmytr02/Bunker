using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIController : StateMachine.StateMachine
{
    public GameObject PousePanel;
    public Settings SettingsPanel;
    public EventTrigger SettingsButton;
    public CommandManager commandManager;
    public Animator loadingScreenAnimator;
    
    public static UIController instance;

    private void Awake()
    {
        if(instance) Destroy(this);
        else instance = this;
    }

    private void Start()
    {
        Begin(new UIGameState(this));
    }
    
    public void MainMenu()
    {
        PhotonNetwork.LeaveRoom();
        loadingScreenAnimator.SetTrigger("loadScene"); 
    }
}
