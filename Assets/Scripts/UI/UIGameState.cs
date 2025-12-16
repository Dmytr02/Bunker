using Unity.VisualScripting;
using UnityEngine;

public class UIGameState : StateMachine.State
{
    public UIGameState(UIController stateMachine) : base(stateMachine) { }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stateMachine.SetState(new UIPouseState(stateMachine as UIController));
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            stateMachine.SetState(new UIChatState(stateMachine as UIController));
        }
    }
    override public void Enter()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    override public void Exit()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
