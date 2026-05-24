using UnityEngine;

public class UIChatState : StateMachine.State<UIController>
{
    public UIChatState(UIController stateMachine) : base(stateMachine) { }

    override public void Enter()
    {
        stateMachine.commandManager.ShowChatPanel();
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
