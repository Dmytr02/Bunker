using UnityEngine;

public class UIPouseState : StateMachine.State
{
    public UIPouseState(UIController stateMachine) : base(stateMachine) { }

    override public void Enter()
    {
        (stateMachine as UIController).PousePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) stateMachine.Dispose();
    }

    override public void Exit()
    {
        (stateMachine as UIController).PousePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
