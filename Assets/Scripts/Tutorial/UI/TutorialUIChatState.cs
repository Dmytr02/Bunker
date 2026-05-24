using UnityEngine;

public class TutorialUIChatState : StateMachine.State<TutorialUIController>
{
    public TutorialUIChatState(TutorialUIController stateMachine) : base(stateMachine) { }

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
        stateMachine.commandManager.HideChatPanel();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
