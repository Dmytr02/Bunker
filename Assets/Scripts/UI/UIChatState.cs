using UnityEngine;

public class UIChatState : StateMachine.State
{
    public UIChatState(UIController stateMachine) : base(stateMachine) { }

    override public void Enter()
    {
        (stateMachine as UIController).commandManager.ShowChatPanel();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) stateMachine.Dispose();
    }

    override public void Exit()
    {
        (stateMachine as UIController).commandManager.HideChatPanel();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
