using Unity.VisualScripting;
using UnityEngine;

public class UIGameState : StateMachine.State<UIController>
{
    public UIGameState(UIController stateMachine) : base(stateMachine) { }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stateMachine.SetState(new UIPouseState(stateMachine));
        }
        
        if (Input.GetKeyDown(stateMachine.chatKey.key))
        {
            stateMachine.SetState(new UIChatState(stateMachine));
        }

        if (Input.GetKeyDown(stateMachine.cameraKey.key))
        {
            stateMachine.isFollowCursore = !stateMachine.isFollowCursore;
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
