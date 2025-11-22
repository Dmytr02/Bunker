using System;
using UnityEngine;

public class UIController : StateMachine.StateMachine
{
    public GameObject PousePanel;
    public CommandManager commandManager;
    
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
}
